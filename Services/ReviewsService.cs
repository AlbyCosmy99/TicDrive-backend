using Microsoft.EntityFrameworkCore;
using TicDrive.Context;
using TicDrive.Dto.ReviewDto;
using TicDrive.Models;

namespace TicDrive.Services
{
    public interface IReviewsService
    {
        Task<List<FullReviewDto>> GetAllReviewsByWorkshopId(string workshopId, int skip, int take);
    }

    public class ReviewsService(TicDriveDbContext context) : IReviewsService
    {
        private readonly TicDriveDbContext _context = context;

        public async Task<List<FullReviewDto>> GetAllReviewsByWorkshopId(string workshopId, int skip, int take)
        {
            return await _context.Reviews
                .Where(review => review.Workshop.Id == workshopId)
                .Include(review => review.Customer)
                .Include(review => review.Workshop)
                .Skip(skip)
                .Take(take)
                .Select(review => new FullReviewDto 
                { 
                    CustomerId = review.CustomerId!,
                    CustomerImageUrl = review.Customer.ProfileImageUrl ?? string.Empty,
                    WorkshopId = review.WorkshopId,
                    Text = review.Text,
                    WhenPublished = review.WhenPublished,
                    Stars = review.Stars

})
                .ToListAsync();
        }
    }
}
