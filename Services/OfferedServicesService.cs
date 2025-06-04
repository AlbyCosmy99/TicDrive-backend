using System.Data.Entity;
using TicDrive.Context;
using TicDrive.Dto.OfferedServicesDto;
using TicDrive.Models;

namespace TicDrive.Services
{
    public interface IOfferedServicesService
    {
        List<OfferedServices> GetOfferedServices(string workshopId, int? serviceId);
    }

    public class OfferedServicesService : IOfferedServicesService
    {
        private readonly TicDriveDbContext _dbContext;
        public OfferedServicesService(TicDriveDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<OfferedServices> GetOfferedServices(string workshopId, int? serviceId)
        {
            var offeredServices = _dbContext.OfferedServices
                .Where(os => os.Workshop.Id == workshopId && os.Active);

            if (serviceId != null)
            {
                offeredServices = offeredServices.Where(os => os.Service.Id == serviceId);
            }

            return offeredServices
                .ToList();
        }
    }
}
