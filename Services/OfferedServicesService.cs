using TicDrive.Context;

namespace TicDrive.Services
{
    public interface IOfferedServicesService
    {
        
    }

    public class OfferedServicesService : IOfferedServicesService
    {
        private readonly TicDriveDbContext _dbContext;
        public OfferedServicesService(TicDriveDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
