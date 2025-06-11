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

namespace ReservationSystem.Infrastructure.Repositories.HotelRepositories
{
    public class HotelAvailabilityRepository
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

        public async Task<List<HotelAvailability>> GetAvailability(HotelBedsSearchRequestModel requestModel)
        {
            var returnModel = new List<HotelAvailability>();

            try
            {
                
               // string action = Environment.GetEnvironmentVariable("n");
                var _url = "https://api.test.hotelbeds.com/hotel-api/v1/hotels";// Environment.GetEnvironmentVariable(amadeusSettings["AmadeusSoap:ApiUrl"]);
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
                        var request = new HttpRequestMessage(HttpMethod.Get, requestModel.ToString());
                       // request.Headers.Add("Authorization", "Bearer " + token);

                        var response = await httpClient.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            try
                            {
                                returnModel = JsonConvert.DeserializeObject<List<HotelAvailability>>(responseContent);
                            }
                            catch(Exception ex)
                            {

                            }
                           
                            string ResToSave = JsonConvert.SerializeObject(returnModel, Newtonsoft.Json.Formatting.Indented);
                           // availabilities.data = result.data;
                            //if (result.data.Count > 0)
                            //{
                            //    if (flightsDictionary != null && flightsDictionary.FirstOrDefault()?.ApplyMarkup == true)
                            //    {
                            //        result.data = applyMarkup(result.data, flightsDictionary);
                            //    }
                            //    if (flightsDictionary != null && flightsDictionary.FirstOrDefault()?.ApplyAirlineDiscount == true)
                            //    {
                            //        result.data = applyDiscount(result.data, flightsDictionary);
                            //    }
                            //    _cacheService.Set("amadeusRequest" + amadeusRequest, availabilities, TimeSpan.FromMinutes(15));
                            //    _dbRepository.SaveAvailabilityResult(amadeusRequest, ResToSave?.ToString(), result.data.Count);
                            //}

                            Console.WriteLine("Response: " + responseContent);
                        }
                        else
                        {
                           // availabilities.amadeusError = new AmadeusResponseError();
                           // availabilities.amadeusError.error = response.StatusCode.ToString();
                           // availabilities.amadeusError.errorCode = 400;
                            if (response.StatusCode.ToString() == "Unauthorized")
                            {
                                _cacheService.Remove("amadeusToken");
                             //   availabilities.amadeusError.errorCode = 401;
                            }
                            var error = await response.Content.ReadAsStringAsync();
                            ErrorResponseAmadeus errorResponse = JsonConvert.DeserializeObject<ErrorResponseAmadeus>(error);

                           // availabilities.amadeusError.error = response.StatusCode.ToString();
                           // availabilities.amadeusError.error_details = errorResponse;
                            Console.WriteLine("Error: " + response.StatusCode);
                        }
                    }
                }
                catch (WebException ex)
                {
                    using (StreamReader rd = new StreamReader(ex.Response.GetResponseStream()))
                    {
                       
                        try
                        {
                            await _dbRepository.SaveAvailabilityResult(System.Text.Json.JsonSerializer.Serialize(requestModel), System.Text.Json.JsonSerializer.Serialize(returnModel), 0);
                        }
                        catch (Exception ex2)
                        {
                            Console.Write($"Error while saving Availibilty log{ex2.Message.ToString()}");
                        }
                        return returnModel;

                    }
                }
            }
            catch (Exception ex)
            {
              //  returnModel.amadeusError = new AmadeusResponseError();
              //  returnModel.amadeusError.error = ex.Message.ToString();
               // returnModel.amadeusError.errorCode = 0;
                return returnModel;
            }


            return returnModel;
        }

    }
}
