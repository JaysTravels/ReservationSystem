using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using ReservationSystem.Domain.DB_Models;
using ReservationSystem.Domain.DBContext;
using ReservationSystem.Domain.Models.Enquiry;
using ReservationSystem.Domain.Models.Insurance;
using ReservationSystem.Domain.Repositories;
using ReservationSystem.Domain.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Infrastructure.Repositories
{
   public class InsuranceRepository : IInsuranceRepository
    {
        private readonly IConfiguration configuration;
        private readonly IHelperRepository _helperRepository;
        private readonly IEmailService _emailService;
        private readonly DB_Context _dbRepository;
        public InsuranceRepository(IConfiguration _configuration, IMemoryCache cache, IEmailService emailService, ICacheService cacheService, DB_Context dBRepository)
        {
            configuration = _configuration;
            _emailService = emailService;
            _dbRepository = dBRepository;
        }

        public async Task<bool> SendInsuranceReqest(InsuranceRequest requestModel)
        {
            try
            {
                #region Save Enquiry To Database
                InsuranceInfo ins = new InsuranceInfo
                {
                    CreatedOn = DateTime.UtcNow,
                    Email = requestModel.Email,
                    WhereTo = requestModel.WhereTo,
                    DepartureDate = requestModel.DepartureDate,
                    ReturnDate = requestModel.ReturnDate,
                    NumberOfTravellers = requestModel.NumnerOfTravellers,
                    Contact = requestModel.Contact
                };
                await _dbRepository.InsuranceInfos.AddAsync(ins);
                await _dbRepository.SaveChangesAsync();
                #endregion

                #region Email Sending Region
                string MsgBoday = await _emailService.GetInsuranceTemplate(requestModel);
                string AdminEmail = "bookings@jaystravels.co.uk";
                if (configuration["EmailSettings:AdminEmail"] != null)
                {
                    AdminEmail = configuration["EmailSettings:AdminEmail"].ToString();
                }
                await _emailService.SendEmailAsync3(AdminEmail, "Insurance Query From Customer", MsgBoday);
                await _emailService.SendEmailAsync3(requestModel.Email, "Insurance Query", MsgBoday);
                #endregion
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while saving request {ex.Message.ToString()}");
            }
            return false;
        }
    }
}
