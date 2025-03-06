using Microsoft.AspNetCore.Mvc;
using TicDrive.Services;

namespace TicDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkshopsController : ControllerBase
    {
        private readonly IWorkshopsService _workshopsService;
        private readonly IAuthService _authService;

        public WorkshopsController(IWorkshopsService workshopsService , IAuthService authService)
        {
            _workshopsService = workshopsService;
            _authService = authService;
        }

        public class GetWorkshopsQueries
        {
            public int Skip { get; set; } = 0;
            public int Take { get; set; } = 10;
            public int ServiceId { get; set; } = 0;
            public string? Filter { get; set; } = string.Empty;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetWorkshops([FromQuery] GetWorkshopsQueries query)
        {
            int skip = query.Skip;
            int take = query.Take;
            int serviceId = query.ServiceId;

            var userClaims = _authService.GetUserClaims(this);
            var userId = _authService.GetUserId(userClaims);

            try
            {
                var workshops = await _workshopsService.GetWorkshops(skip, take, serviceId, userId, filter: query.Filter);

                return Ok(new { workshops = workshops.Skip(skip).Take(take).ToList(), Count = workshops.Count() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
