using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TicDrive.Context;
using TicDrive.Models;

namespace TicDrive.Services
{
    public interface IServicesService
    {
        Task<List<Service>> GetServices(string workshopId);
    }

    public class ServicesService : IServicesService
    {
        private readonly TicDriveDbContext _dbContext;

        public ServicesService(TicDriveDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Service>> GetServices(string workshopId)
        {
            var query = _dbContext.Services.AsQueryable();

            if (!workshopId.IsNullOrEmpty())
            {
                query = query.Join(
                    _dbContext.OfferedServices.Where(os => os.Workshop.Id == workshopId),
                    service => service.Id,
                    offeredService => offeredService.Service.Id,
                    (service, offeredService) => service
                );
            }

            return await query.OrderBy(service => service.Id).ToListAsync();
        }
    }
}
