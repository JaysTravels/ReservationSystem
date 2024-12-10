using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using ReservationSystem.Domain.Models.Payment;
using ReservationSystem.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace ReservationSystem.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IConfiguration configuration;
        private readonly IMemoryCache _cache;
        private readonly IHelperRepository _helperRepository;
        public PaymentRepository(IConfiguration _configuration, IMemoryCache cache, IHelperRepository helperRepository)
        {
            configuration = _configuration;
            _cache = cache;
            _helperRepository = helperRepository;
        }

        public async Task<PaymentResponse> GeneratePaymentRequest(PaymentRequest request)
        {
            PaymentResponse response = new PaymentResponse();
            try
            {
                var _settings = configuration.GetSection("EpdqSettings");
                string OrderId = Guid.NewGuid().ToString().Substring(0, 13);
                string strPw = _settings["ShaInPassphrase"]?.ToString();
                string planDigest =
                    "ACCEPTURL=" + _settings["AcceptUrl"] + strPw +
                    "AMOUNT=" + (request.Amount).ToString() + strPw +
                    "CANCELURL=" + _settings["CancelUrl"] + strPw +
                    "CN=" + "jays" + strPw +
                    "CURRENCY=" + request.Currency + strPw +
                    "DECLINEURL=" + _settings["DeclineUrl"] + strPw +
                     "EMAIL=" + "jays@jays.co.uk" + strPw +
                    "EXCEPTIONURL=" + _settings["EXCEPTIONURL"] + strPw +
                    "LANGUAGE=" + request.Language + strPw +
                    "ORDERID=" + OrderId + strPw +
                    "PSPID=" + _settings["PSPID"] + strPw;

             string  shaSignature = Sha1HasData(planDigest);
                var parameters = new Dictionary<string, string>
            {
                    { "ACCEPTURL", _settings["AcceptUrl"] },
                    { "AMOUNT", (request.Amount).ToString() },
                    { "CANCELURL", _settings["CancelUrl"] },
                    { "CN", "Jays" },
                    { "CURRENCY", request.Currency },
                    { "DECLINEURL", _settings["DeclineUrl"] },
                    { "EMAIL", "jays@jays.co.uk" },
                    { "EXCEPTIONURL", _settings["ExceptionUrl"] },
                    { "LANGUAGE", request.Language },
                    { "ORDERID",OrderId},
                    { "PSPID", _settings["PSPID"] },                     
            };
            // string shaSignature = GenerateShaSignature(parameters, _settings["ShaInPassphrase"]);
            parameters.Add("SHASIGN", shaSignature);
            response.Parameters = parameters;
            response.Url = _settings["PaymentUrl"];
            }
            catch(Exception ex)
            {
                Console.WriteLine($" Error in Create Payment Request {ex.Message.ToString()}");
            }
            return response;
        }
        private string Sha1HasData(string data)
        {
            SHA1 Hasher = SHA1.Create();
            byte[] NCodetxt = Encoding.Default.GetBytes(data);
            byte[] HashedDataBytes = Hasher.ComputeHash(NCodetxt);
            StringBuilder HashedDataStringBuld = new StringBuilder();
            for(int i=0;i< HashedDataBytes.Length; i++)
            {
                HashedDataStringBuld.Append(HashedDataBytes[i].ToString("X2"));
            }
            return HashedDataStringBuld.ToString();
        }

        private string GenerateShaSignature(Dictionary<string, string> parameters, string passphrase)
        {
            try
            {
                var sortedParams = parameters.OrderBy(p => p.Key);
                var sb = new StringBuilder();

                foreach (var param in sortedParams)
                {
                    if (!string.IsNullOrEmpty(param.Value))
                    {
                        sb.Append(param.Key).Append('=').Append(param.Value).Append(passphrase);
                    }
                }

                using var sha256 = SHA256.Create();
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToUpper();

            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error While Generate Sha Signature {ex.Message.ToString()}");
                return "";
            }
           
        }

        public async Task<bool> VerifyShaOutSignature(Dictionary<string, string> form)
        {
            try
            {
                var _settings = configuration.GetSection("EpdqSettings");
                string passphrase = _settings["ShaOutPassphrase"];
                var sortedParams = form
                                .Where(p => !string.IsNullOrEmpty(p.Value) && p.Key != "SHASIGN")
                                .OrderBy(p => p.Key);

                var sb = new StringBuilder();
                foreach (var param in sortedParams)
                {
                    sb.Append(param.Key).Append('=').Append(param.Value).Append(passphrase);
                }
                using var sha256 = SHA256.Create();
                var computedHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
                var computedSignature = BitConverter.ToString(computedHash).Replace("-", string.Empty).ToUpper();

                return computedSignature == form.GetValueOrDefault("SHASIGN");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error while Verify Shah out Signature {ex.Message.ToString()}");
                return false;
            }
            
        }

    }
}
