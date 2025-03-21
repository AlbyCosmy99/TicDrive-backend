using System.Data.Entity;
using TicDrive.Context;
using TicDrive.Models;
using static TicDrive.Controllers.CarsController;

namespace TicDrive.Services
{
    public interface ICarsService
    {
        List<CarMake> GetMakes();
        List<CarModel> GetCarModelsByMakeId(int makeId);
        Task<bool> PostCar(AddCarQuery query, string customerId);
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

        public async Task<bool> PostCar(AddCarQuery query, string customerId)
        {
            var car = _dbContext.Cars.Where(car => car.LicencePlate == query.Plate).FirstOrDefault();

            if(car == null)
            {
                var carMake = _dbContext.CarMakes.Where(car => car.Name == query.Make).FirstOrDefault();

                if (carMake == null)
                {
                    return false;
                }

                var carModel = _dbContext.CarModels.Where(car => car.CarMakeId == carMake.Id && car.Name == query.Model && car.Year == query.Year).FirstOrDefault();

                if (carModel == null)
                {
                    return false;
                }

                var newCar = new Car
                {
                    LicencePlate = query.Plate,
                    CarModelId = carModel.Id,
                    Year = query.Year,
                    FuelType = query.FuelType,
                    TransmissionType = query.TransmissionType,
                    EngineDisplacement = query.EngineDisplacement,
                };

                _dbContext.Cars.Add(newCar);
                await _dbContext.SaveChangesAsync();

                car = _dbContext.Cars.Where(car => car.LicencePlate == query.Plate).FirstOrDefault();
            }

            if(car == null)
            {
                return false;
            }

            var customerCar = _dbContext.CustomerCar.Where(c => c.CustomerId == customerId && c.CarId == car.Id).FirstOrDefault();

            if(customerCar != null) //car already registered for this user
            {
                return false;
            }

            var newCustomerCar = new CustomerCar
            {
                CarId = car.Id,
                CustomerId = customerId,
                Name = query.Name != null ? query.Name : "My Car",
                Km = query.Km
            };

            _dbContext.CustomerCar.Add(newCustomerCar);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
