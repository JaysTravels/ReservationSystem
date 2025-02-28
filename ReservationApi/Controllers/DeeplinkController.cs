using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ReservationApi.Model;
using ReservationSystem.Domain.Models.AirSellFromRecommendation;
using ReservationSystem.Domain.Models.Deeplink;
using ReservationSystem.Domain.Repositories;
using ReservationSystem.Domain.Service;

namespace ReservationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeeplinkController : ControllerBase
    {
        private IDeeplinkRepository _Repo;
        private ICacheService _cacheService;
        private readonly IMemoryCache _cache;
        public DeeplinkController(IDeeplinkRepository Repo, IMemoryCache memoryCache, ICacheService cacheService)
        {
            _Repo = Repo;
            _cache = memoryCache;
            _cacheService = cacheService;
        }
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DeeplinkRequest request)
        {

            ApiResponse res = new ApiResponse();

            var data = await _Repo.GetDeepLink();

            res.IsSuccessful = data?.Error == null ? true : false;
            res.StatusCode = data?.Error == null ? 200 : 500;
            res.Message = data?.Error == null ? "Found Success: Total records:" + data.deepLinkModels.Count() : "Error";
            res.Response = data?.Error == null ? "Success" : "Failed";            
            res.Data = data;           

            return Ok(res);

        }
    }
}
