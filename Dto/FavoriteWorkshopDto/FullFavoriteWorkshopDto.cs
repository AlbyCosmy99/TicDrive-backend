using TicDrive.Models;

namespace TicDrive.Dto.FavoriteWorkshopDto
{
    public class FullFavoriteWorkshopDto
    {
        public int Id { get; set; }
        public required string CustomerId { get; set; }
        public required string WorkshopId { get; set; }
    }
}
