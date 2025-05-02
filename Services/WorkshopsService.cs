using Microsoft.EntityFrameworkCore;
using TicDrive.Context;
using TicDrive.Dto.UserDto;
using TicDrive.Dto.UserDto.WorkshopDto;
using TicDrive.Enums;
using TicDrive.Models;
using TicDrive.Utils.Location;

namespace TicDrive.Services
{
    public interface IWorkshopsService
    {
        Task<IEnumerable<WorkshopDashboardInfoDto>> GetWorkshops(int skip, int take, int? serviceId = 0, string? customerId = null, bool? favorite = null, string? filter = null);
        Task LikeWorkshop(string userId, string workshopId);
        Task<List<NearbyWorkshopDto>> GetNearbyWorkshops(int skip, int take, decimal latitude, decimal longitude, int? serviceId, int? kmRange = 20, string? filter = null);
    }

    public class WorkshopsService(TicDriveDbContext context) : IWorkshopsService
    {
        private readonly TicDriveDbContext _context = context;

        public async Task<IEnumerable<WorkshopDashboardInfoDto>> GetWorkshops(int skip, int take, int? serviceId, string? customerId, bool? favorite = false, string? filter = null)
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

            if (favorite == true)
            {
                uniqueWorkshops = uniqueWorkshops.Where(workshop => workshop.IsFavorite == true);
            }

            return uniqueWorkshops
                .Where(workshop => workshop.Name.ToLower().Contains(filter?.ToLower() ?? string.Empty));
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

        public async Task<List<NearbyWorkshopDto>> GetNearbyWorkshops(int skip, int take, decimal latitude, decimal longitude, int? serviceId, int? kmRange = 20, string? filter = null)
        {
            if (serviceId != null)
            {
                var workshopsWithService = await _context.Users
                    .Where(user => 
                        user.UserType == UserType.Workshop 
                        && user.Latitude != null 
                        && user.Longitude != null
                        && (filter == null || (user.Name.Contains(filter))))
                    .Join(
                        _context.OfferedServices,
                        workshop => workshop.Id,
                        service => service.WorkshopId,
                        (workshop, service) => new { Workshop = workshop, Service = service }
                    )
                    .Where(joined => joined.Service.ServiceId == serviceId)
                    .ToListAsync();

                var nearbyWorkshops = workshopsWithService
                    .Where(joined =>
                    {
                        double distanceInMeters = DistanceCalculator.CalculateDistanceInMeters(
                            (double)latitude,
                            (double)longitude,
                            (double)joined.Workshop.Latitude!,
                            (double)joined.Workshop.Longitude!
                        );
                        return distanceInMeters <= (kmRange ?? 20) * 1000;
                    })
                    .Select(joined => new NearbyWorkshopDto
                    {
                        Id = joined.Workshop.Id,
                        Name = joined.Workshop.Name!,
                        Address = joined.Workshop.Address,
                        Latitude = joined.Workshop.Latitude,
                        Longitude = joined.Workshop.Longitude,
                        ProfileImageUrl = joined.Workshop.ProfileImageUrl,
                        ServicePrice = joined.Service.Price,
                        Currency = joined.Service.Currency,
                        Discount = joined.Service.Discount
                    })
                    .Skip(skip)
                    .Take(take)
                    .ToList();

                return nearbyWorkshops;
            }
            else
            {
                var allWorkshops = await _context.Users
                     .Where(user =>
                        user.UserType == UserType.Workshop
                        && user.Latitude != null
                        && user.Longitude != null
                        && (filter == null || (user.Name.Contains(filter))))
                    .ToListAsync();

                var nearbyWorkshops = allWorkshops
                    .Where(workshop =>
                    {
                        double distanceInMeters = DistanceCalculator.CalculateDistanceInMeters(
                            (double)latitude,
                            (double)longitude,
                            (double)workshop.Latitude!,
                            (double)workshop.Longitude!
                        );
                        return distanceInMeters <= (kmRange ?? 20) * 1000;
                    })
                    .Select(workshop => new NearbyWorkshopDto
                    {
                        Id = workshop.Id,
                        Name = workshop.Name!,
                        Address = workshop.Address,
                        Latitude = workshop.Latitude,
                        Longitude = workshop.Longitude,
                        ProfileImageUrl = workshop.ProfileImageUrl,
                        ServicePrice = null,
                        Currency = null,
                        Discount = null
                    })
                    .Skip(skip)
                    .Take(take)
                    .ToList();

                return nearbyWorkshops;
            }
        }
    }
}
