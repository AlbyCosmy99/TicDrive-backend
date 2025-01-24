using Microsoft.AspNetCore.Mvc;

namespace TicDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfferedServicesController : ControllerBase
    {
        public class GetOfferedServicesQueries
        {
            public required string WorkshopId { get; set; }
            public required int ServiceId { get; set; }
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetOfferedServices([FromQuery] GetOfferedServicesQueries query)
        {
            if (query == null || string.IsNullOrEmpty(query.WorkshopId) || query.ServiceId == 0)
            {
                return BadRequest("WorkshopId and ServiceId are required as query parameters.");
            }

            return Ok(new { Message = "Services retrieved successfully." });
        }
    }
}
