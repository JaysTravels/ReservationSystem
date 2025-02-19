using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReservationApi.Model;
using ReservationSystem.Domain.Models.Enquiry;
using ReservationSystem.Domain.Models.Insurance;
using ReservationSystem.Domain.Repositories;

namespace ReservationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InsuranceController : ControllerBase
    {
        private IInsuranceRepository _repo;

        public InsuranceController(IInsuranceRepository repo)
        {
            _repo = repo;
        }

        //[Authorize]
        [HttpPost("sendinsurance")]
        public async Task<IActionResult> Post([FromBody] InsuranceRequest request)
        {

            ApiResponse res = new ApiResponse();
            var data = await _repo.SendInsuranceReqest(request);
            res.IsSuccessful = data;
            res.StatusCode = data ? 200 : 500;
            res.Message = data ? "Send Email Success" : "Faile to send email";
            res.Response = data ? "Send Email Success" : "Faile to send email";
            res.Data = data;
            return Ok(res);

        }

    }
}
