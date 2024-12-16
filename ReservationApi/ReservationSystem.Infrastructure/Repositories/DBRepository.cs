using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ReservationApi.ReservationSystem.Domain.DB_Models;
using ReservationSystem.Domain.DB_Models;
using ReservationSystem.Domain.DBContext;
using ReservationSystem.Domain.Models;
using ReservationSystem.Domain.Models.AddPnrMulti;
using ReservationSystem.Domain.Models.DBLogs;
using ReservationSystem.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace ReservationSystem.Infrastructure.Repositories
{
    public class DBRepository : IDBRepository
    {
        private DB_Context _context;
        private readonly ILogger<DBRepository> _logger;
        public DBRepository(IConfiguration configuration, DB_Context context, ILogger<DBRepository> logger)
        {
           _context = context;
            _logger = logger;
        }

        public async Task SaveAvailabilityResult(string? requset , string? response , int totlResults)
        {
            try
            {
                var Res = new SearchAvailabilityResults();
                Res.created_on = DateTime.Now.ToUniversalTime();
                Res.request = requset;
                Res.response = response;
                Res.total_results = totlResults;
                Res.user_id = 0;
                await _context.availabilityResults.AddAsync(Res);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error while saving Search Result {ex.Message.ToString()}");
            }
          
           
        }

        private string GetJsonRequest(string Envelope)
        {
            try{
                XmlDocument xmlReq = new XmlDocument();
                xmlReq.LoadXml(Envelope);
                return JsonConvert.SerializeXmlNode(xmlReq, Newtonsoft.Json.Formatting.Indented);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while conver request to json {ex.Message.ToString()}");
                return Envelope;
            }
        }
        public async Task SaveReservationFlow(SaveReservationLog requset)
        {
            try
            {
                var Res = new ReservationFlow();
                Res.CreatedOn = DateTime.Now.ToUniversalTime();
                Res.Request = GetJsonRequest(requset.Request);
                Res.Response = requset.Response;
                Res.RequestName = requset.RequestName;
                Res.AmadeusSessionId = requset.AmadeusSessionId;
                Res.UserId = 0;                
                await _context.reservation_flow.AddAsync(Res);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while saving Search Result {ex.Message.ToString()}");
            }


        }

        public async Task SavePassengerInfo(AddPnrMultiRequset request)
        {
            try
            {
                
                foreach(var passenger in request.passengerDetails)
                {
                    var Res = new PassengerInfo();
                    Res.CreatedOn = DateTime.Now.ToUniversalTime();
                    Res.SessionId = request.sessionDetails.SessionId;
                    Res.FirstName = passenger.firstName;
                    Res.LastName = passenger.surName;
                    Res.PassengerType = passenger.type;
                    Res.PhoneNumber = passenger?.phone;
                    Res.Email = passenger?.email;
                    Res.DOB = passenger?.dob;
                    Res.IsLead = passenger?.isLeadPassenger;
                    
                    await _context.PassengersInfo.AddAsync(Res);
                    await _context.SaveChangesAsync();
                }
              
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while saving Passenger Details {ex.Message.ToString()}");
            }


        }
        public async Task<List<FlightMarkup>?> GetFlightMarkup()
        {
            try
            {

                return await _context.flightMarkups.ToListAsync();
            }
            catch (Exception ex)
            {
             
                _logger.LogError($"Error return flights markup Result {ex.Message.ToString()}");
                return null;
            }


        }

        public async Task SaveBookingInfo( PnrCommitRequest request , string error , string pnrNumber)
        {

            try
            {
                var Res = new BookingInfo();
                Res.CreatedOn = DateTime.UtcNow;
                Res.SessionId = request.sessionDetails.SessionId;
                Res.FirstName = request.FirstName;
                Res.LastName = request.LastName;
                Res.PnrNumber = pnrNumber;
                Res.TotalAmount = request.TotalAmount;               
                Res.Error = error;
                await _context.BookingInfo.AddAsync(Res);
                await _context.SaveChangesAsync();               

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while saving Passenger Details {ex.Message.ToString()}");
            }
        }
    }
}
