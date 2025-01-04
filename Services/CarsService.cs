using TicDrive.Context;
using TicDrive.Models;

namespace TicDrive.Services
{
    public interface ICarsService
    {
        List<CarMake> GetMakes();
    }
    public class CarsService : ICarsService
    {
        private readonly TicDriveDbContext _dbContext;
        public CarsService(TicDriveDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<CarMake> GetMakes() {
            return _dbContext.CarMakes.ToList();
        }

    }
}
