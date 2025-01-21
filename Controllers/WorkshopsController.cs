using Microsoft.AspNetCore.Mvc;
using TicDrive.Services;

namespace TicDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkshopsController : ControllerBase
    {
        private readonly IWorkshopsService _workshopsService;
        public WorkshopsController(IWorkshopsService workshopsService) { 
            _workshopsService = workshopsService;
        }
        public class GetWorkshopsQueries
        {
            public int Skip { get; set; }
            public int Take { get; set; }
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetWorkshops([FromQuery] GetWorkshopsQueries query)
        {
            int skip = 0;
            int take = 10;
            if (query != null && query.Skip > 0)
            {
                skip = query.Skip;
            }
            if (query != null && query.Take > 0)
            {
                take = query.Take;
            }

            return Ok(_workshopsService.GetWorkshops(skip, take));
        }
    }
}
