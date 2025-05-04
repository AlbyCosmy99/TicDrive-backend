using Microsoft.AspNetCore.Mvc;
using TicDrive.Context;

namespace TicDrive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DateTimeController : ControllerBase
    {
        private readonly TicDriveDbContext _dbContext;

        public DateTimeController(TicDriveDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("days")]
        public IActionResult GetDays()
        {
            var days = _dbContext.Days.ToList();
            return Ok(days);
        }

    }
}
