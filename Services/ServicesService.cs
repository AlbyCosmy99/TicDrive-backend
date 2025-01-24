using Microsoft.EntityFrameworkCore;
using TicDrive.Context;
using TicDrive.Models;

namespace TicDrive.Services
{
    public interface IServicesService
    {
        Task<List<Service>> GetServices();
    }
    public class ServicesService : IServicesService
    {
        private readonly TicDriveDbContext _dbContext;
        public ServicesService(TicDriveDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Service>> GetServices()
        {
            return await _dbContext.Services.OrderBy(service => service.Id).ToListAsync();
        }
    }
}
