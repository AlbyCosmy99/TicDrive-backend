using TicDrive.Models;

namespace TicDrive.Dto.ReviewDto
{
    public class FullReviewDto
    {
        public int Id { get; set; }
        public required string CustomerId { get; set; }
        public string CustomerImageUrl { get; set; } = string.Empty;
        public required string WorkshopId { get; set; }
        public required string Text { get; set; }
        public DateTime WhenPublished { get; set; } = DateTime.Now;

        public required double Stars { get; set; }
    }
}
