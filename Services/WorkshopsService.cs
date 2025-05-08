using System;
using Autofac.Features.OwnedInstances;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicDrive.Context;
using TicDrive.Dto.UserDto;
using TicDrive.Dto.UserDto.WorkshopDto;
using TicDrive.Dto.UserImageDto;
using TicDrive.Enums;
using TicDrive.Models;
using TicDrive.Utils.Location;

namespace TicDrive.Services
{
    public interface IWorkshopsService
    {
        Task<IEnumerable<WorkshopDashboardInfoDto>> GetWorkshops(int? serviceId, string? customerId, decimal? latitude, decimal? longitude, bool? favorite = false, string? filter = null, int? kmRange = 20);
        Task LikeWorkshop(string userId, string workshopId);
        List<Specialization> GetSpecializations();
    }

    public class WorkshopsService : IWorkshopsService
    {
        private readonly TicDriveDbContext _context;
        private readonly IImagesService _imagesService;
        private readonly IMapper _mapper;

        public WorkshopsService(TicDriveDbContext context, IImagesService imagesService, IMapper mapper)
        {
            _context = context;
            _imagesService = imagesService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WorkshopDashboardInfoDto>> GetWorkshops(int? serviceId, string? customerId, decimal? latitude, decimal? longitude, bool? favorite = false, string? filter = null, int? kmRange = 20)
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

            if (serviceId != 0 && serviceId != null)
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
                .Select(group => group.First());

            if(latitude != null && longitude != null && favorite != true)
            {
                uniqueWorkshops = uniqueWorkshops
                   .Where(joined =>
                   {
                       double distanceInMeters = DistanceCalculator.CalculateDistanceInMeters(
                           (double)latitude,
                           (double)longitude,
                           (double)joined.Workshop.Latitude!,
                           (double)joined.Workshop.Longitude!
                       );
                       return distanceInMeters <= (kmRange ?? 20) * 1000;
                   });
            }

            Dictionary<string, List<FullUserImageDto>> imagesDict = [];
            foreach (var data in uniqueWorkshops)
            {
                var images = await _imagesService.GetUserImagesAsync(data.Workshop.Id,1);
                imagesDict[data.Workshop.Id] = _mapper.Map<List<FullUserImageDto>>(images);
            }

            var projectedWorkshops = uniqueWorkshops
                .Select(joined => new WorkshopDashboardInfoDto
                {
                    Id = joined.Workshop.Id,
                    Name = joined.Workshop.Name,
                    Address = joined.Workshop.Address,
                    Latitude = joined.Workshop.Latitude,
                    Longitude = joined.Workshop.Longitude,
                    MeanStars = joined.Reviews.Any() ? joined.Reviews.Average(review => review.Stars) : 0,
                    NumberOfReviews = joined.Reviews.Count(),
                    ServicePrice = serviceId != 0 && joined.OfferedService != null ? joined.OfferedService.Price : null,
                    Currency = serviceId != 0 && joined.OfferedService != null ? joined.OfferedService.Currency : null,
                    Discount = serviceId != 0 && joined.OfferedService != null ? joined.OfferedService.Discount : null,
                    IsVerified = joined.Workshop.IsVerified,
                    IsFavorite = string.IsNullOrEmpty(customerId) ? null : favoriteWorkshopIds.Contains(joined.Workshop.Id),
                    Images = imagesDict[joined.Workshop.Id]
                });

            if (favorite == true)
            {
                projectedWorkshops = projectedWorkshops.Where(workshop => workshop.IsFavorite == true);
            }

            return projectedWorkshops
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

        public List<Specialization> GetSpecializations()
        {
            return _context.Specializations.ToList();
        }
    }
}
