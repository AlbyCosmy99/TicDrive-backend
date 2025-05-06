namespace TicDrive.Dto.UserDto.WorkshopDto
{
    public class NearbyWorkshopDto
    {
        public string Id { get; set; }
        public required string Name { get; set; }
        public string? Address { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? ServicePrice { get; set; }
        public char? Currency { get; set; } = '€';
        public decimal? Discount { get; set; }
    }
}
