using TicDrive.Models;

namespace TicDrive.Dto.CarDto.CarModelDto
{
    public class FullCarModelDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int Year { get; set; }
        public required int CarMakeId { get; set; }
    }
}
