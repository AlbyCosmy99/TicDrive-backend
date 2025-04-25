namespace TicDrive.Dto.UserDto
{
    public class FullUserDto
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
}
