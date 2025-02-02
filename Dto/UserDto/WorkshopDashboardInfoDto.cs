using TicDrive.Enums;

namespace TicDrive.Dto.UserDto
{
    public class WorkshopDashboardInfoDto
    {
        public string Id { get; set; }
        public required string Name { get; set; }
        public string? Address { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public string? ProfileImageUrl { get; set; }
        public double MeanStars { get; set; }
        public int NumberOfReviews { get; set; }
        public decimal? ServicePrice { get; set; }
        public char? Currency { get; set; } = '€';
        public decimal? Discount { get; set; }
        public bool? IsVerified { get; set; }
    }
}
