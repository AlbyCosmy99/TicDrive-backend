using TicDrive.Enums;

namespace TicDrive.Dto.UserDto
{
    public class WorkshopDashboardInfoDto
    {
        public string Id { get; set; }
        public required string Name { get; set; }
        public UserType UserType { get; set; }
        public string? Address { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public double MeanStars { get; set; }
        public int NumberOfReviews { get; set; }
    }
}
