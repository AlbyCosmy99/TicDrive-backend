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

    public class ReviewsService(TicDriveDbContext context, IImagesService imagesService) : IReviewsService
    {
        private readonly TicDriveDbContext _context = context;
        private readonly IImagesService _imagesService = imagesService;

        public async Task<List<FullReviewDto>> GetAllReviewsByWorkshopId(string workshopId, int skip, int take)
        {
            var reviews = _context.Reviews
                .Where(review => review.Workshop.Id == workshopId)
                .Include(review => review.Customer)
                .Include(review => review.Workshop)
                .Skip(skip)
                .Take(take)
                .ToList();

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
                    CustomerImageUrl = image != null ? image.Filename : string.Empty,
                    WorkshopId = review.WorkshopId,
                    Text = review.Text,
                    WhenPublished = review.WhenPublished,
                    Stars = review.Stars
                });
            }

            return fullReviewDtos;
        }

    }
}
