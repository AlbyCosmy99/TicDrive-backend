using Microsoft.EntityFrameworkCore;
using TicDrive.Context;
using TicDrive.Dto.CarDto.CustomerCarDto;
using TicDrive.Models;
using static TicDrive.Controllers.CarsController;

namespace TicDrive.Services
{
    public interface ICarsService
    {
        List<CarMake> GetMakes();
        List<CarModel> GetCarModelsByMakeId(int makeId);
        Task<bool> PostCustomerCar(AddCarQuery query, string customerId);
        Task UpdateCustomerCar(AddCarQuery query, string customerId);
        List<FullCustomerCarDto> GetCustomerCars(string customerId);
        Task DeleteCustomerCar(int carId);
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

        public async Task<bool> PostCustomerCar(AddCarQuery query, string customerId)
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
                    CV = query.CV
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

            if(customerCar != null)
            {
                return false; //car already registered for this user
            }

            var newCustomerCar = new CustomerCar
            {
                CarId = car.Id,
                CustomerId = customerId,
                Name = query.Name != null ? query.Name : "My Car",
                Km = query.Mileage
            };

            _dbContext.CustomerCars.Add(newCustomerCar);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public List<FullCustomerCarDto> GetCustomerCars(string customerId)
        {
            return _dbContext.CustomerCars
                .Where(customerCar => customerCar.CustomerId == customerId)
                .Join(_dbContext.Cars,
                customerCar => customerCar.CarId,
                car => car.Id,
                (customerCar, car) => new { customerCar, car })
                .Join(_dbContext.CarModels,
                ccc => ccc.car.CarModelId,
                model => model.Id,
                (ccc,model) => new {ccc, model})
                .Join(_dbContext.CarMakes,
                cccm => cccm.model.CarMakeId,
                make => make.Id,
                (cccm, make) => new FullCustomerCarDto
                {
                    Id = cccm.ccc.customerCar.Id,
                    Make = make.Name,
                    Model = cccm.model.Name,
                    PlateNumber = cccm.ccc.car.LicencePlate!,
                    Year = cccm.model.Year,
                    EngineDisplacement = cccm.ccc.car.EngineDisplacement,
                    FuelType = cccm.ccc.car.FuelType,
                    Mileage = cccm.ccc.customerCar.Km,
                    CarName = cccm.ccc.customerCar.Name,
                    CV = cccm.ccc.customerCar.Car.CV,
                    CustomerId = cccm.ccc.customerCar.CustomerId
                })
                .ToList();
        }

        public async Task UpdateCustomerCar(AddCarQuery query, string customerId)
        {
            var customerCar = await _dbContext.CustomerCars
                .FirstOrDefaultAsync(cc => cc.CustomerId == customerId && cc.CarId == query.Id)
                ?? throw new Exception("Car not found.");

            if (query.Mileage != null)
                customerCar.Km = query.Mileage;

            if (!string.IsNullOrWhiteSpace(query.Name))
                customerCar.Name = query.Name;

            var car = await _dbContext.Cars
                .FirstOrDefaultAsync(c => c.Id == customerCar.CarId)
                ?? throw new Exception("Associated car not found.");

            if (!string.IsNullOrWhiteSpace(query.Plate))
                car.LicencePlate = query.Plate;

            if (query.FuelType != null)
                car.FuelType = query.FuelType;

            if (query.TransmissionType != null)
                car.TransmissionType = query.TransmissionType;

            if (query.EngineDisplacement != null)
                car.EngineDisplacement = query.EngineDisplacement;

            if (!string.IsNullOrWhiteSpace(query.Make) && !string.IsNullOrWhiteSpace(query.Model))
            {
                var carMake = await _dbContext.CarMakes
                    .FirstOrDefaultAsync(m => m.Name == query.Make)
                    ?? throw new Exception("Car make not found.");

                var carModel = await _dbContext.CarModels
                    .FirstOrDefaultAsync(m => m.Name == query.Model && m.CarMakeId == carMake.Id)
                    ?? throw new Exception("Car model not found.");

                car.CarModelId = carModel.Id;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteCustomerCar(int carId)
        {
            var car = await _dbContext.CustomerCars
                .FirstOrDefaultAsync(c => c.Id == carId);

            if (car == null)
            {
                throw new InvalidOperationException($"Car with ID {carId} not found.");
            }

            _dbContext.CustomerCars.Remove(car);
            await _dbContext.SaveChangesAsync();
        }

    }
}
