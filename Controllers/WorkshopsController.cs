using Microsoft.AspNetCore.Mvc;
using TicDrive.Services;

namespace TicDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkshopsController : ControllerBase
    {
        private readonly IWorkshopsService _workshopsService;

        public WorkshopsController(IWorkshopsService workshopsService)
        {
            _workshopsService = workshopsService;
        }

        public class GetWorkshopsQueries
        {
            public int Skip { get; set; } = 0;
            public int Take { get; set; } = 10;
            public int serviceId { get; set; } = 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkshops([FromQuery] GetWorkshopsQueries query)
        {
            int skip = query.Skip;
            int take = query.Take;
            int serviceId = query.serviceId;

            try
            {
                var workshops = await _workshopsService.GetWorkshops(skip, take, serviceId);

                return Ok(workshops);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
