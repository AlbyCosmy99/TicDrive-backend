using Microsoft.EntityFrameworkCore;
using TicDrive.Context;
using TicDrive.Dto.UserDto;

namespace TicDrive.Services
{
    public interface IWorkshopsService
    {
        Task<List<WorkshopDashboardInfoDto>> GetWorkshops(int skip, int take, int? serviceId = 0, string? customerId = null, bool? favorite = null);
        Task LikeWorkshop(string userId, string workshopId);
    }

    public class WorkshopsService(TicDriveDbContext context) : IWorkshopsService
    {
        private readonly TicDriveDbContext _context = context;

        public async Task<List<WorkshopDashboardInfoDto>> GetWorkshops(int skip, int take, int? serviceId, string? customerId, bool? favorite = false)
        {
            var workshopsQuery = _context.Users
                .Where(user => user.UserType == Enums.UserType.Workshop)
                .GroupJoin(
                    _context.Reviews,
                    workshop => workshop.Id,
                    review => review.Workshop.Id,
                    (workshop, reviews) => new { Workshop = workshop, Reviews = reviews }
                )
                .SelectMany(
                    group => _context.OfferedServices
                        .Where(offeredService => offeredService.Workshop.Id == group.Workshop.Id)
                        .DefaultIfEmpty(),
                    (group, offeredService) => new { group.Workshop, group.Reviews, OfferedService = offeredService }
                );

            if (serviceId != 0)
            {
                workshopsQuery = workshopsQuery
                    .Where(joined => joined.OfferedService != null && joined.OfferedService.Service.Id == serviceId);
            }

            List<string> favoriteWorkshopIds = new List<string>();
            if (!string.IsNullOrEmpty(customerId))
            {
                favoriteWorkshopIds = await _context.FavoriteWorkshops
                    .Where(f => f.CustomerId == customerId)
                    .Select(f => f.WorkshopId)
                .ToListAsync();
            }

            var workshopsData = await workshopsQuery.ToListAsync();

            var uniqueWorkshops = workshopsData
                .GroupBy(joined => joined.Workshop.Id)
                .Select(group => group.First())
                .Select(joined => new WorkshopDashboardInfoDto
                {
                    Id = joined.Workshop.Id,
                    Name = joined.Workshop.Name,
                    Address = joined.Workshop.Address,
                    Latitude = joined.Workshop.Latitude,
                    Longitude = joined.Workshop.Longitude,
                    ProfileImageUrl = joined.Workshop.ProfileImageUrl,
                    MeanStars = joined.Reviews.Any() ? joined.Reviews.Average(review => review.Stars) : 0,
                    NumberOfReviews = joined.Reviews.Count(),
                    ServicePrice = serviceId != 0 && joined.OfferedService != null ? joined.OfferedService.Price : null,
                    Currency = serviceId != 0 && joined.OfferedService != null ? joined.OfferedService.Currency : null,
                    Discount = serviceId != 0 && joined.OfferedService != null ? joined.OfferedService.Discount : null,
                    IsVerified = joined.Workshop.IsVerified,
                    IsFavorite = string.IsNullOrEmpty(customerId) ? null : favoriteWorkshopIds.Contains(joined.Workshop.Id)
                });

            if(favorite == true)
            {
                uniqueWorkshops = uniqueWorkshops.Where(workshop => workshop.IsFavorite == true);
            }          

            return uniqueWorkshops
                .Skip(skip)
                .Take(take)
                .ToList();
        }
        public async Task LikeWorkshop(string userId, string workshopId)
        {
            var favoriteWorkshop = await _context.FavoriteWorkshops
                   .FirstOrDefaultAsync(f => f.CustomerId == userId && f.WorkshopId == workshopId);

            if (favoriteWorkshop != null)
            {
                _context.FavoriteWorkshops.Remove(favoriteWorkshop);
            } 
            else
            {
                await _context.FavoriteWorkshops.AddAsync(new Models.FavoriteWorkshop
                {
                    CustomerId = userId,
                    WorkshopId = workshopId
                });
            }

            await _context.SaveChangesAsync();
        }

    }
}
