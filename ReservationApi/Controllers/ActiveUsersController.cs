using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ReservationApi.Model;
using ReservationSystem.Domain.Models.ActiveUsers;
using ReservationSystem.Domain.Models.AirSellFromRecommendation;
using ReservationSystem.Domain.Repositories;
using ReservationSystem.Domain.Service;

namespace ReservationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActiveUsersController : ControllerBase
    {

        private IActiveUsersRepository _Repo;
        private ICacheService _cacheService;
        private readonly IMemoryCache _cache;

        public ActiveUsersController(IActiveUsersRepository Repo, IMemoryCache memoryCache, ICacheService cacheService)
        {
            _Repo = Repo;
            _cache = memoryCache;
            _cacheService = cacheService;
        }

        //[Authorize]
        [HttpPost("trackusers")]
        public async Task<IActionResult> Post([FromBody] ActiveUserRequest request)
        {

            ApiResponse res = new ApiResponse();

            var data = await _Repo.TrackUsers(request);

            res.IsSuccessful = data?.ToString() == "OK" ? true : false;
            res.StatusCode = data?.ToString() == "OK" ? 200 : 500;
            res.Message = data?.ToString() == "OK" ? "Update Success:"  : "Error";
            res.Response = data?.ToString() == "OK" ? "Success" : "Failed";
            res.Data = data;           
            return Ok(res);

        }

        //[Authorize]
        [HttpPost("clearusers")]
        public async Task<IActionResult> PostClearUser([FromBody] ActiveUserRequest request)
        {

            ApiResponse res = new ApiResponse();

            var data = await _Repo.ClearUsers();

            res.IsSuccessful = data?.ToString() == "OK" ? true : false;
            res.StatusCode = data?.ToString() == "OK" ? 200 : 500;
            res.Message = data?.ToString() == "OK" ? "Clear Success:" : "Error";
            res.Response = data?.ToString() == "OK" ? "Success" : "Failed";
            res.Data = data;
            return Ok(res);

        }

    }
}
