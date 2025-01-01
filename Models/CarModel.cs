namespace TicDrive.Models
{
    public class CarModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int Year { get; set; }
        public required CarMake CarMake { get; set; }
    }
}
