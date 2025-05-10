using TicDrive.Context;
using TicDrive.Dto.DateTimeDto;
using TicDrive.Models.DateTime;
using TicDrive.Models.Workshops;

namespace TicDrive.Services
{
    public interface IDateTimeService
    {
        WorkshopNotAvailableDaysDto GetWorkshopNotAvailableDays(string workshopId);
    }
    public class DateTimeService : IDateTimeService
    {
        private readonly TicDriveDbContext _dbContext;

        public DateTimeService(TicDriveDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public WorkshopNotAvailableDaysDto GetWorkshopNotAvailableDays(string workshopId)
        {
            var workingDays = _dbContext.WorkshopsSchedules
                .Where(workshopSchedule => workshopSchedule.WorkshopId == workshopId)
                .Join(
                    _dbContext.Days,
                    workshopSchedule => workshopSchedule.DayId,
                    day => day.Id,
                    (workshopSchedule, day) => new { workshopSchedule, day }
                )
                .Select(wsd => new FullDayDto
                {
                    Id = wsd.day.Id,
                    Name = wsd.day.Name
                })
                .ToList();

            var days = _dbContext.Days.ToList();

            var nonWorkingDays = days
                .Where(day => !workingDays.Any(wd => wd.Id == day.Id))
                .Select(day => new FullDayDto
                {
                    Id = day.Id,
                    Name = day.Name
                })
                .ToList();

            var notWorkingDates = _dbContext.WorkshopsNonWorkingDays
                .Where(wnwd => wnwd.WorkshopId == workshopId)
                .Select(wnwd => wnwd.Date)
                .ToList();  

            return new WorkshopNotAvailableDaysDto
            {
                WorkshopId = workshopId,
                Days = nonWorkingDays,
                Dates = notWorkingDates
            };
        }

    }
}
