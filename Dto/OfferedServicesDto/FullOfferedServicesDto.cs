namespace TicDrive.Dto.OfferedServicesDto
{
    public class FullOfferedServicesDto
    {
        public required int Id { get; set; }
        public required int ServiceId { get; set; }
        public required string WorkshopId { get; set; }
        public required decimal Price { get; set; }
        public required char Currency { get; set; } = '€';
        public decimal? Discount { get; set; } = 0;
    }
}
