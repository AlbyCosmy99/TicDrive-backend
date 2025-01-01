namespace TicDrive.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public required string LicencePlate { get; set; }
        public required CarModel CarModel { get; set; }
        public required User Owner { get; set; }
    }
}
