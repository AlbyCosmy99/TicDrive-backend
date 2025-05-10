using TicDrive.Models.DateTime;

namespace TicDrive.Dto.DateTimeDto
{
    public class WorkshopNotAvailableDaysDto
    {
        public string WorkshopId { get; set; }
        public List<FullDayDto> Days { get; set; }
        public List<DateTime> Dates { get; set; }   
    }
}
