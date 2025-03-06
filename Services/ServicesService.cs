using TicDrive.Context;
using TicDrive.Models;

namespace TicDrive.Services
{
    public interface IServicesService
    {
        IQueryable<Service> GetServices(string workshopId, string? filter = null);
    }

    public class ServicesService : IServicesService
    {
        private readonly TicDriveDbContext _dbContext;

        public ServicesService(TicDriveDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Service> GetServices(string workshopId, string? filter = null)
        {
            var query = _dbContext.Services.AsQueryable();

            if (!string.IsNullOrEmpty(workshopId))
            {
                query = query.Join(
                    _dbContext.OfferedServices.Where(os => os.Workshop.Id == workshopId),
                    service => service.Id,
                    offeredService => offeredService.Service.Id,
                    (service, offeredService) => service
                );
            }

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(service => service.Title.Contains(filter));
            }

            return query.OrderBy(service => service.Id);
        }
    }
}
