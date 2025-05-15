using Microsoft.AspNetCore.Mvc;
using TicDrive.Context;
using TicDrive.Models;
using TicDrive.Services;

namespace TicDrive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DateTimeController : ControllerBase
    {
        private readonly TicDriveDbContext _dbContext;
        private readonly IDateTimeService _dateTimeService;

        public DateTimeController(TicDriveDbContext dbContext, IDateTimeService dateTimeService)
        {
            _dbContext = dbContext;
            _dateTimeService = dateTimeService;
        }

        public class GetDaysQuery
        {
            public string? LanguageCode { get; set; }
        }

        [HttpGet("days")]
        public IActionResult GetDays([FromQuery] GetDaysQuery query)
        {
            var code = query?.LanguageCode?.ToLower() ?? "en";

            var days = _dbContext.DaysTranslations
                .Join(_dbContext.Languages.Where(language => language.Code == code),
                    dayTranslation => dayTranslation.LanguageId,
                    language => language.Id,
                    (dayTranslation, language) => new
                    {
                        dayTranslation.DayId,
                        dayTranslation.LanguageId,
                        dayTranslation.Label
                    });

            return Ok(days);
        }

        public class GetWorkshopNotAvailableDaysQuery
        {
            public string WorkshopId { get; set; }
        }

        [HttpGet("workshop/notAvailableDays")]
        public IActionResult GetWorkshopNotAvailableDays([FromQuery] GetWorkshopNotAvailableDaysQuery query)
        {

            return Ok(_dateTimeService.GetWorkshopNotAvailableDays(query.WorkshopId));
        }

        public class GetWorkshopWorkingHoursQuery
        {
            public string Day { get; set; }
            public string WorkshopId { get; set; }
        }

        [HttpGet("workshop/workingHours")]
        public IActionResult GetWorkshopWorkingHours([FromQuery] GetWorkshopWorkingHoursQuery query)
        {
            if(query == null || query.Day == null || query.WorkshopId == null)
            {
                return BadRequest("The day name and workshop id are necessary.");
            }
            
            try
            {
                return Ok(_dateTimeService.GetWorkshopWorkingHours(query.WorkshopId, query.Day));
            } catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
