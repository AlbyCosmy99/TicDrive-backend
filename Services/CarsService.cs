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
        Task PostCar(AddCarQuery query, string customerId);
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

        public async Task PostCar(AddCarQuery query, string customerId)
        {
            var car = _dbContext.Cars.Where(car => car.LicencePlate == query.Plate).FirstOrDefault();

            if(car == null)
            {
                var carMake = _dbContext.CarMakes.Where(car => car.Name == query.Make).FirstOrDefault();

                if (carMake == null)
                {
                    throw new Exception("Car make is null.");
                }

                var carModel = _dbContext.CarModels.Where(car => car.CarMakeId == carMake.Id && car.Name == query.Model && car.Year == query.Year).FirstOrDefault();

                if (carModel == null)
                {
                    throw new Exception("Car model is null.");
                }

                var newCar = new Car
                {
                    LicencePlate = query.Plate,
                    CarModelId = carModel.Id,
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
                throw new Exception("Error while registering the car.");
            }

            var customerCar = _dbContext.CustomerCars.Where(c => c.CustomerId == customerId && c.CarId == car.Id).FirstOrDefault();

            if(customerCar != null) //car already registered for this user
            {
                throw new Exception("Car already registered for this user.");
            }

            var newCustomerCar = new CustomerCar
            {
                CarId = car.Id,
                CustomerId = customerId,
                Name = query.Name != null ? query.Name : "My Car",
                Km = query.Km
            };

            _dbContext.CustomerCars.Add(newCustomerCar);
            await _dbContext.SaveChangesAsync();
        }
    }
}
