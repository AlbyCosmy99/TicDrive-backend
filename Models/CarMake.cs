namespace TicDrive.Models
{
    public class CarMake
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public ICollection<CarModel> CarModels { get; set; } = [];
    }
}
