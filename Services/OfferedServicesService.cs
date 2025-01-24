using TicDrive.Context;
using TicDrive.Models;

namespace TicDrive.Services
{
    public interface IOfferedServicesService
    {
        // Add method signatures for the operations you plan to support
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
