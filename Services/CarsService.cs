using System.Data.Entity;
using TicDrive.Context;
using TicDrive.Models;

namespace TicDrive.Services
{
    public interface ICarsService
    {
        List<CarMake> GetMakes();
        List<CarModel> GetCarModelsByMakeId(int makeId);
    }
    public class CarsService : ICarsService
    {
        private readonly TicDriveDbContext _dbContext;
        public CarsService(TicDriveDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<CarMake> GetMakes() {
            return [.. _dbContext.CarMakes];
        }

        public List<CarModel> GetCarModelsByMakeId(int makeId)
        {
            return [.. _dbContext.CarModels.Where(model => model.CarMakeId == makeId)];
        }
    }
}
