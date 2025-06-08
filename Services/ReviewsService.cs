using Microsoft.EntityFrameworkCore;
using TicDrive.Context;
using TicDrive.Dto.ReviewDto;
using TicDrive.Models;
using TicDrive.Utils.DateTime;

namespace TicDrive.Services
{
    public interface IReviewsService
    {
        Task<List<FullReviewDto>> GetAllReviewsByWorkshopId(string workshopId, int skip, int take);
    }

    public class ReviewsService(TicDriveDbContext context, IImagesService imagesService) : IReviewsService
    {
        private readonly TicDriveDbContext _context = context;
        private readonly IImagesService _imagesService = imagesService;

        public async Task<List<FullReviewDto>> GetAllReviewsByWorkshopId(string workshopId, int skip, int take)
        {
            var reviews = await _context.Reviews
                .Where(review => review.WorkshopId == workshopId)
                .Include(review => review.Customer)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            var fullReviewDtos = new List<FullReviewDto>();

            foreach (var review in reviews)
            {
                var customerImageUrl = await _imagesService.GetUserImagesAsync(review.CustomerId!, 1);
                var image = customerImageUrl.FirstOrDefault();

                fullReviewDtos.Add(new FullReviewDto
                {
                    Id = review.Id,
                    CustomerId = review.CustomerId!,
                    CustomerName = review.Customer.Name,
                    CustomerImageUrl = image,
                    WorkshopId = review.WorkshopId,
                    Text = review.Text,
                    WhenPublished = DateTimeFormatter.FormatLocalDateInItalian(review.WhenPublished),
                    Stars = review.Stars
                });
            }

            return fullReviewDtos;
        }
    }

}
