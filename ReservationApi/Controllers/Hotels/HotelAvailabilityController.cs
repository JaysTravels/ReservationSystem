using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ReservationApi.Model;
using ReservationSystem.Domain.Models.Availability;
using ReservationSystem.Domain.Models.Hotels.AvailabilityResponsee;
using ReservationSystem.Domain.Models.Hotels.AvailabiltiyRequest;
using ReservationSystem.Domain.Repositories;
using ReservationSystem.Domain.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ReservationApi.Controllers.Hotels
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelAvailabilityController : ControllerBase
    {
        private IHotelAvailailityRepository _availability;
        private ICacheService _cacheService;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly IHelperRepository _helperRepository;
        public HotelAvailabilityController(IHotelAvailailityRepository availability, IMemoryCache memoryCache, ICacheService cacheService, IConfiguration configuration, IHelperRepository helperRepository)
        {
            _availability = availability;
            _cache = memoryCache;
            _cacheService = cacheService;
            _configuration = configuration;
            _helperRepository = helperRepository;
        }

        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] HotelAvailabilityRequest availabilityRequest)
        {

            ApiResponse res = new ApiResponse();

            var data = await _availability.GetAvailability(availabilityRequest);

            res.IsSuccessful = data?.error  == null ? true : false;
            res.StatusCode = data?.error == null ? 200 : 500;
            res.Message = data?.error == null ? "Found Success: Total records:" + data.HotelResponse?.hotels?.hotels.Count() : "Error";
            res.Response = data?.error == null ? "Success" : "Failed";
            if (data?.error != null)
            {
                res.Data = data?.error;
                res.StatusCode =  500;
            }
            else
            {
                res.Data = data;
            }

            return Ok(res);

        }

       

    }
}
