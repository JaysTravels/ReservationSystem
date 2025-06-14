using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ReservationSystem.Domain.DBContext;
using ReservationSystem.Domain.Models.Availability;
using ReservationSystem.Domain.Models;
using ReservationSystem.Domain.Repositories;
using ReservationSystem.Domain.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using ReservationSystem.Domain.Models.Hotels.AvailabilityResponsee;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using ReservationSystem.Domain.Models.Hotels.AvailabiltiyRequest;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

namespace ReservationSystem.Infrastructure.Repositories.HotelRepositories
{
    public class HotelAvailabilityRepository : IHotelAvailailityRepository
    {
        private readonly IConfiguration configuration;
        private readonly ICacheService _cacheService;
        private readonly IDBRepository _dbRepository;
        private readonly IHelperRepository _helperRepository;

        public HotelAvailabilityRepository(IConfiguration _configuration, ICacheService cacheService, IDBRepository dBRepository, IHelperRepository helperRepository)
        {
            configuration = _configuration;
            _cacheService = cacheService;
            _dbRepository = dBRepository;
            _helperRepository = helperRepository;
        }

        public async Task<HotelAvailabilityResponse> GetAvailability(HotelAvailabilityRequest requestModel)
        {
            var returnModel = new HotelAvailabilityResponse();

            try
            {
                
                var _url = "https://api.test.hotelbeds.com/hotel-api/v1/hotels";
                var token = _helperRepository.GetHotelToken();
                var apikey = _helperRepository.GetHotelApiKey();
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "application/gzip");
                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Api-key", apikey);
                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Signature", token);
                        var json = System.Text.Json.JsonSerializer.Serialize(requestModel);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await httpClient.PostAsync(_url, content);
                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            try
                            {
                                returnModel.HotelResponse  = JsonConvert.DeserializeObject<HotelResponse>(responseContent);
                            }
                            catch (Exception ex)
                            {

                            }

                            string ResToSave = JsonConvert.SerializeObject(returnModel, Newtonsoft.Json.Formatting.Indented);
                        }
                        else
                        {
                            var responseContentError = await response.Content.ReadAsStringAsync();
                            returnModel.error = new Domain.Models.Hotels.AvailabilityResponsee.Error();
                            returnModel.error.errorText = responseContentError;

                        }
                        }
                    }
                catch (WebException ex)
                {
                    using (StreamReader rd = new StreamReader(ex.Response.GetResponseStream()))
                    {
                       
                        try
                        {
                        //    await _dbRepository.SaveAvailabilityResult(System.Text.Json.JsonSerializer.Serialize(requestModel), System.Text.Json.JsonSerializer.Serialize(returnModel), 0);
                        }
                        catch (Exception ex2)
                        {
                            Console.Write($"Error while saving Availibilty log{ex2.Message.ToString()}");
                        }
                        returnModel.error = new Domain.Models.Hotels.AvailabilityResponsee.Error();
                        returnModel.error.errorText = rd.ToString();
                        return returnModel;

                    }
                }
            }
            catch (Exception ex)
            {
                returnModel.error = new Domain.Models.Hotels.AvailabilityResponsee.Error();
                returnModel.error.errorText = ex.Message.ToString();
                return returnModel;
            }


            return returnModel;
        }

       
    }
}
