using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ReservationApi.Model;
using ReservationSystem.Domain.Models.Availability;
using ReservationSystem.Domain.Repositories;
using ReservationSystem.Domain.Service;
using System.Text;
using System.Xml.Linq;

namespace ReservationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleFlightsController : ControllerBase
    {
        private ITravelBoardSearchRepository _availability;
        private ICacheService _cacheService;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly IHelperRepository _helperRepository;
        private readonly IGoogleFlightsRepository _googleRepository;
        public GoogleFlightsController(ITravelBoardSearchRepository availability, IMemoryCache memoryCache, ICacheService cacheService, IConfiguration configuration, IHelperRepository helperRepository, IGoogleFlightsRepository googleRepository)
        {
            _availability = availability;
            _cache = memoryCache;
            _cacheService = cacheService;
            _configuration = configuration;
            _helperRepository = helperRepository;
            _googleRepository = googleRepository;
        }

        //[Authorize]
        [HttpPost]       
        public async Task<IActionResult> Post()
        {

            ApiResponse res = new ApiResponse();
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var flightRequest = await reader.ReadToEndAsync();


            var availabilityRequest = await _googleRepository.GetFlightRequeust(flightRequest);
            var data = await _availability.GetAvailability(availabilityRequest);
            if(data?.amadeusError == null)
            {
                var Responsne = await _googleRepository.CreateXmlFeed(availabilityRequest, data);
                res.Data = Responsne.ToString();
            }
        

            res.IsSuccessful = data?.amadeusError == null ? true : false;
            res.StatusCode = data?.amadeusError == null ? 200 : 500;
            res.Message = data?.amadeusError == null ? "Found Success: Total records:" + data.data.ToList().Count() : data?.amadeusError.error.ToString();
            res.Response = data?.amadeusError == null ? "Success" : "Failed";
           
            return Ok(res);

        }

        //[Authorize]
        [HttpGet("GetFlight")]       
        public async Task<IActionResult> GetFlight(string flightId)
        {
            ApiResponse res = new ApiResponse();
            var data = await _googleRepository.GetFlightFromCache(flightId);
            res.Data = data;
            res.IsSuccessful = data != null ? true : false;
            res.StatusCode = data != null ? 200 : 500;
            res.Message = data != null ? "Success"  : "Error";
            res.Response = data != null ? "Success" : "Error";
            return Ok(res);

        }
    }
}
