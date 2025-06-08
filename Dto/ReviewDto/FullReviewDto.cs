using TicDrive.Models;

namespace TicDrive.Dto.ReviewDto
{
    public class FullReviewDto
    {
        public int Id { get; set; }
        public required string CustomerId { get; set; }
        public required string CustomerName { get; set; }
        public UserImage CustomerImageUrl { get; set; }
        public required string WorkshopId { get; set; }
        public required string Text { get; set; }
        public string WhenPublished { get; set; } = string.Empty;

        public required double Stars { get; set; }
    }
}
