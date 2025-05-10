using Microsoft.AspNetCore.Mvc;
using TicDrive.Context;
using TicDrive.Models;

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

        public class GetDaysQuery
        {
            public string? LanguageCode { get; set; }
        }

        [HttpGet]
        [Route("days")]
        public IActionResult GetDays([FromQuery] GetDaysQuery query)
        {
            var code = "en";
            if(query != null && query.LanguageCode != null)
            {
                code = query.LanguageCode;
            }
            var days = _dbContext.DaysTranslations
                .Join(_dbContext.Languages
                    .Where(language => language.Code == code.ToLower()),
                dayTranslation => dayTranslation.LanguageId,
                language => language.Id,
                (dayTranslation, language) => new { dayTranslation, language })
                .Select(day => new
                {
                    day.dayTranslation.Id,
                    day.dayTranslation.LanguageId,
                    day.dayTranslation.Label

                });
            return Ok(days);
        }

    }
}
