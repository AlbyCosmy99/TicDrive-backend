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
            _dbContext.Services.Add(new Service { Id = 1, Title = "TEST" });
            _dbContext.SaveChanges();
            return _dbContext.Services.ToList();
        }
    }
}
