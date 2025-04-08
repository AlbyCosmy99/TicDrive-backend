using TicDrive.Enums;

namespace TicDrive.Dto.CarDto.CustomerCarDto
{
    public class FullCustomerCarDto
    {
        public int Id { get; set; } 
        public required string CustomerId { get; set; }
        public required string Make { get; set; }
        public required string Model { get; set; }
        public required string PlateNumber { get; set; }
        public int Year { get; set; }
        public string? EngineDisplacement { get; set; }
        public FuelType? FuelType { get; set; }
        public int? Mileage { get; set; }
        public string? CarName { get; set; }
        public int? CV { get; set; }
    }
}
