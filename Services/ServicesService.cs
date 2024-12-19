using TicDrive.Context;
using TicDrive.Models;

namespace TicDrive.Services
{
    public interface IServicesService
    {
        List<Service> GetServices();
    }
    public class ServicesService : IServicesService
    {
        private readonly TicDriveDbContext _dbContext;
        public ServicesService(TicDriveDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<Service> GetServices()
        {
            return _dbContext.Services.ToList();
        }
    }
}
