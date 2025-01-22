using TicDrive.Context;
using TicDrive.Dto.UserDto;

namespace TicDrive.Services
{
    public interface IWorkshopsService
    {
        List<WorkshopDashboardInfoDto> GetWorkshops(int skip, int take);
    }
    public class WorkshopsService: IWorkshopsService
    {
        private readonly TicDriveDbContext _context;
        public WorkshopsService(TicDriveDbContext context) {
            _context = context;
        }
        public List<WorkshopDashboardInfoDto> GetWorkshops(int skip, int take)
        {
            var workshops = _context.Users
             .Where(user => user.UserType == Enums.UserType.Workshop)
             .GroupJoin(
                 _context.Reviews,
                 workshop => workshop.Id,
                 review => review.Workshop.Id,
                 (workshop, reviews) => new { Workshop = workshop, Reviews = reviews }
             )
             .Select(group => new WorkshopDashboardInfoDto
             {
                 Id = group.Workshop.Id,
                 UserType = group.Workshop.UserType,
                 Name = group.Workshop.Name,
                 Address = group.Workshop.Address,
                 Latitude = group.Workshop.Latitude,
                 Longitude = group.Workshop.Longitude,
                 ProfileImageUrl = group.Workshop.ProfileImageUrl,
                 MeanStars = group.Reviews.Any() ? group.Reviews.Average(review => review.Stars) : 0,
                 NumberOfReviews = group.Reviews.Count()
             })
             .Skip(skip)
             .Take(take)
             .ToList();

            return workshops;
        }
    }
}
