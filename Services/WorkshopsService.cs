using Microsoft.EntityFrameworkCore;
using TicDrive.Context;
using TicDrive.Dto.UserDto;

namespace TicDrive.Services
{
    public interface IWorkshopsService
    {
        Task<List<WorkshopDashboardInfoDto>> GetWorkshops(int skip, int take, int serviceId);
    }

    public class WorkshopsService(TicDriveDbContext context) : IWorkshopsService
    {
        private readonly TicDriveDbContext _context = context;

        public async Task<List<WorkshopDashboardInfoDto>> GetWorkshops(int skip, int take, int serviceId)
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
                    IsVerified = joined.Workshop.IsVerified
                })
                .Skip(skip)
                .Take(take)
                .ToList();

            return uniqueWorkshops;
        }
    }
}
