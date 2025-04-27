using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ReservationApi.Model;
using ReservationSystem.Domain.Models.Availability;
using ReservationSystem.Domain.Repositories;
using ReservationSystem.Domain.Service;

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
        public GoogleFlightsController(ITravelBoardSearchRepository availability, IMemoryCache memoryCache, ICacheService cacheService, IConfiguration configuration, IHelperRepository helperRepository)
        {
            _availability = availability;
            _cache = memoryCache;
            _cacheService = cacheService;
            _configuration = configuration;
            _helperRepository = helperRepository;
        }

        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string flightRequest)
        {

            ApiResponse res = new ApiResponse();

            // var data = await _availability.GetAvailability(availabilityRequest);

            //res.IsSuccessful = data?.amadeusError == null ? true : false;
            //res.StatusCode = data?.amadeusError == null ? 200 : 500;
            //res.Message = data?.amadeusError == null ? "Found Success: Total records:" + data.data.ToList().Count() : "Error";
            //res.Response = data?.amadeusError == null ? "Success" : "Failed";
            //if (data?.amadeusError != null)
            //{
            //    res.Data = data?.amadeusError;
            //    res.StatusCode = data?.amadeusError?.errorCode.Value != 0 ? data.amadeusError.errorCode.Value : 500;
            //}
            //else
            //{
            //    res.Data = data.data;
            //}

            return Ok();

        }
    }
}
