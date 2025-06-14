﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Newtonsoft.Json;
using ReservationSystem.Domain.Models.Availability;
using ReservationSystem.Domain.Models.FareCheck;
using ReservationSystem.Domain.Models;
using System.Net;
using ReservationSystem.Domain.DBContext;

namespace ReservationSystem.Infrastructure.Repositories
{
    public class HelperRepository : IHelperRepository
    {
        private readonly IConfiguration configuration;
        private readonly IMemoryCache _cache;
        private readonly DB_Context dB_Context;
        public HelperRepository(IConfiguration _configuration, IMemoryCache cache , DB_Context _db_Context)
        {
            configuration = _configuration;
            _cache = cache;
            dB_Context = _db_Context;
        }
        public async Task SaveJson( string jsonText , string filename)
        {
            try
            {
                var Allowlogs = configuration.GetSection("writelogs")?.Value;
                if (Allowlogs == "true")
                {
                    var logsPath = configuration.GetSection("Logspath").Value;
                    await System.IO.File.WriteAllTextAsync(logsPath + filename + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ".json", jsonText);
                }
                              
            }
            catch (Exception ex)
            {
               
            }
        }
        public async Task SaveXmlResponseold(string filename, string response)
        {
            try
            {
                var allowLogs = configuration.GetSection("writelogs")?.Value;
                if (allowLogs?.ToLower() == "true")
                {
                    // Parse XML with proper error handling
                    XDocument xmlDoc;
                    try
                    {
                        xmlDoc = XDocument.Parse(response);
                    }
                    catch (Exception xmlEx)
                    {
                        Console.WriteLine($"Invalid XML format: {xmlEx.Message}");
                        return; // Exit if XML is invalid
                    }

                    // XML Writer Settings
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = System.Text.Encoding.UTF8
                    };

                    // Determine logs path
                    var logsPath = configuration.GetSection("Logspath")?.Value;

                    if (!string.IsNullOrEmpty(logsPath))
                    {
                        logsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "App_Data", "logs");
                        // Auto-detect environment (Azure or Local)
                        //logsPath = Environment.GetEnvironmentVariable("HOME") != null
                        //    ? Path.Combine(Environment.GetEnvironmentVariable("HOME"), "LogFiles") // Azure App Service
                        //    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"); // Local bin/logs
                    }

                    // Ensure the directory exists
                    if (!Directory.Exists(logsPath))
                    {
                        Directory.CreateDirectory(logsPath);
                    }

                    // Generate a timestamped filename
                    string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                    string fullPath = Path.Combine(logsPath, $"{filename}_{timestamp}.xml");

                    // Write XML to file asynchronously
                    using (XmlWriter writer = XmlWriter.Create(fullPath, settings))
                    {
                        xmlDoc.Save(writer);
                    }

                    Console.WriteLine($"XML log saved successfully: {fullPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while saving XML response: {ex.Message}");
            }
        }

        public async Task SaveXmlResponse(string filename, string response)
        {
            try
            {
                var Allowlogs = configuration.GetSection("writelogs")?.Value;
                if (Allowlogs == "true")
                {
                    XDocument xmlDoc = XDocument.Parse(response);
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = System.Text.Encoding.UTF8
                    };

                    var logsPath = configuration.GetSection("Logspath").Value;
                    using (XmlWriter writer = XmlWriter.Create(logsPath + filename + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ".xml", settings))
                    {
                        xmlDoc.Save(writer);
                    }

                }

            }
            catch ( Exception ex)
            {
                Console.WriteLine($"Error while save xml response {ex.Message.ToString()}");
            }
          
          
        }

        public async Task<string> generatePassword()
        {
            try
            {
                var amadeusSettings = configuration.GetSection("AmadeusSoap");
                string password = Environment.GetEnvironmentVariable(configuration["AmadeusSoap:clearPassword"]);               
                string passSHA;
                byte[] nonce = new byte[32];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(nonce);
                }
                DateTime utcNow = DateTime.UtcNow;
                string TIMESTAMP = utcNow.ToString("o");
                string nonceBase64 = Convert.ToBase64String(nonce);               
                using (SHA1 sha1 = SHA1.Create())
                {
                    byte[] passwordSha = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
                    byte[] combined = Combine(nonce, Encoding.UTF8.GetBytes(TIMESTAMP), passwordSha);
                    byte[] passSha = sha1.ComputeHash(combined);
                    passSHA = Convert.ToBase64String(passSha);
                }
                return passSHA + "|" + nonceBase64 + "|" + TIMESTAMP;

            }
            catch (Exception ex)
            {
                return "Error while generate pwd " + ex.Message.ToString();
            }
        }
        static byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

        public async Task Security_Signout(HeaderSession header)
        {
            FareCheckReturnModel fareCheck = new FareCheckReturnModel();
            try
            {

                var amadeusSettings = configuration.GetSection("AmadeusSoap");
                var _url = amadeusSettings["ApiUrl"];
                var _action = amadeusSettings["Security_SignOut"];
                string Result = string.Empty;
                string Envelope = await Signout_Request(header);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);
                request.Headers.Add("SOAPAction", _action);
                request.ContentType = "text/xml;charset=\"utf-8\"";
                request.Accept = "text/xml";
                request.Method = "POST";

                using (Stream stream = request.GetRequestStream())
                {
                    byte[] content = Encoding.UTF8.GetBytes(Envelope);
                    stream.Write(content, 0, content.Length);
                }

                try
                {
                    using (WebResponse response = request.GetResponse())
                    {
                        using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                        {
                            var result2 = rd.ReadToEnd();
                            XDocument xmlDoc = XDocument.Parse(result2);
                            await SaveXmlResponse("securitySignout", result2);
                            XmlDocument xmlDoc2 = new XmlDocument();
                            xmlDoc2.LoadXml(result2);
                            string jsonText = JsonConvert.SerializeXmlNode(xmlDoc2, Newtonsoft.Json.Formatting.Indented);
                            await SaveJson(jsonText, "securitySignoutJson");
                            XNamespace fareNS = "http://xml.amadeus.com/FARQNR_07_1_1A";
                            var errorInfo = xmlDoc.Descendants(fareNS + "errorInfo").FirstOrDefault();
                            if (errorInfo != null)
                            {
                                var errorCode = errorInfo.Descendants(fareNS + "rejectErrorCode").Descendants(fareNS + "errorDetails").Descendants(fareNS + "errorCode").FirstOrDefault()?.Value;
                                var errorText = errorInfo.Descendants(fareNS + "errorFreeText").Descendants(fareNS + "freeText").FirstOrDefault()?.Value;
                                fareCheck.amadeusError = new AmadeusResponseError();
                                fareCheck.amadeusError.error = errorText;
                                fareCheck.amadeusError.errorCode = Convert.ToInt16(errorCode);

                            }
                        }
                    }
                }
                catch (WebException ex)
                {
                    using (StreamReader rd = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        Result = rd.ReadToEnd();
                        fareCheck.amadeusError = new AmadeusResponseError();
                        fareCheck.amadeusError.error = Result;
                        fareCheck.amadeusError.errorCode = 0;

                    }
                }
            }
            catch (Exception ex)
            {
                fareCheck.amadeusError = new AmadeusResponseError();
                fareCheck.amadeusError.error = ex.Message.ToString();
                fareCheck.amadeusError.errorCode = 0;
                // return fareCheck;
            }
            // return fareCheck;
        }
        public async Task<string> Signout_Request(HeaderSession requestModel)
        {

            var amadeusSettings = configuration.GetSection("AmadeusSoap");
            string action = amadeusSettings["Security_SignOut"];
            string to = amadeusSettings["ApiUrl"];
            string Request = $@"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:ses=""http://xml.amadeus.com/2010/06/Session_v3"">
   <soap:Header xmlns:add=""http://www.w3.org/2005/08/addressing"">
      <ses:Session TransactionStatusCode=""End"">
      <ses:SessionId>{requestModel.SessionId}</ses:SessionId>
      <ses:SequenceNumber>{requestModel.SequenceNumber + 1}</ses:SequenceNumber>
      <ses:SecurityToken>{requestModel.SecurityToken}</ses:SecurityToken>
    </ses:Session>
    <add:MessageID>{System.Guid.NewGuid()}</add:MessageID>
    <add:Action>{action}</add:Action>
    <add:To>{to}</add:To>  
    <link:TransactionFlowLink xmlns:link=""http://wsdl.amadeus.com/2010/06/ws/Link_v1""/>
   </soap:Header>
   <soap:Body>
     <Security_SignOut></Security_SignOut>  
   </soap:Body>
</soap:Envelope>";

            return Request;
        }

        public string GenerateReferenceNumber()
        {
            string prefix = "JAYS-";
            //int remainingLength = 11 - prefix.Length;
            //  const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int seed = DateTime.Today.GetHashCode();
            Random random = new Random(seed);
            HashSet<int> randomNumbers = new HashSet<int>();
            while (randomNumbers.Count < 5)
            {
                randomNumbers.Add(random.Next(1, 100)); // Random numbers between 1 and 100
            }
            StringBuilder randomString = new StringBuilder();
            foreach (int number in randomNumbers)
            {
               // Console.WriteLine(number);
                randomString.Append(number.ToString());
            }
           // var randomString = new string(Enumerable.Repeat(chars, remainingLength).Select(s => s[new Random().Next(s.Length)]).ToArray());
            return prefix + randomString.ToString().Substring(0,5);
        }


        /// Hotel Code ////
        /// 
        static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // format as hexadecimal
                }

                return builder.ToString();
            }
        }

        public string GetHotelToken()
        {
            string apikey = Environment.GetEnvironmentVariable("HotelBedsApiKey"); //   
            string secret = Environment.GetEnvironmentVariable("HotelBedsSecret"); //  
            string timestamp = ((Int32)(DateTimeOffset.UtcNow.ToUnixTimeSeconds())).ToString();

            string dataToHash = apikey + secret + timestamp;
            string signature = ComputeSha256Hash(dataToHash);
            return signature;
        }

        public string GetHotelApiKey()
        {
            string apikey = Environment.GetEnvironmentVariable("HotelBedsApiKey");// 
            return apikey;
        }
    }
}
