namespace TicDrive.Dto.DateTimeDto
{
    public class WorkshopWorkingHoursDto
    {
        public int Id { get; set; }
        public List<TimeOnly?> Morning { get; set; } = [];
        public List<TimeOnly?> Afternoon { get; set; } = [];
    }
}
