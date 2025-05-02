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
        [Consumes("application/xml")]
        [Produces("application/xml")]
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
                return Ok(Responsne.ToString());
            }
        

            res.IsSuccessful = data?.amadeusError == null ? true : false;
            res.StatusCode = data?.amadeusError == null ? 200 : 500;
            res.Message = data?.amadeusError == null ? "Found Success: Total records:" + data.data.ToList().Count() : "Error";
            res.Response = data?.amadeusError == null ? "Success" : "Failed";
           
            return Ok(res);

        }
    }
}
