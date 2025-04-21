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
        List<CarModelVersion> GetModelVersions(int modelId);
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

        public List<CarModelVersion> GetModelVersions(int modelId)
        {
            return [.. _dbContext.CarModelVersions.Where(version => version.CarModelId == modelId)];
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

                var carModel = _dbContext.CarModels.Where(car => car.CarMakeId == carMake.Id && car.Name == query.Model).FirstOrDefault();

                if (carModel == null)
                {
                    throw new Exception("Car model is null.");
                }


                var carVersion = _dbContext.CarModelVersions
                    .Where(carVersion => carVersion.Year == query.Year && carVersion.CarModel == carModel)
                    .FirstOrDefault();

                if (carVersion == null)
                {
                    throw new Exception("Car version is null.");
                }

                var newCar = new Car
                {
                    LicencePlate = query.Plate,
                    CarModelVersionId = carVersion.Id,
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
                .Join(_dbContext.CarModelVersions,
                ccc => ccc.car.CarModelVersionId,
                modelVersion => modelVersion.Id,
                (ccc, modelVersion) => new { ccc, modelVersion })
                .Join(_dbContext.CarModels,
                cccv => cccv.modelVersion.CarModelId,
                model => model.Id,
                (cccv, model) => new { cccv, model})
                .Join(_dbContext.CarMakes,
                cccvm => cccvm.model.CarMakeId,
                make => make.Id,
                (cccvm, make) => new FullCustomerCarDto
                {
                    Id = cccvm.cccv.ccc.customerCar.Id,
                    Make = make.Name,
                    Model = cccvm.model.Name,
                    PlateNumber = cccvm.cccv.ccc.car.LicencePlate!,
                    Year = cccvm.cccv.modelVersion.Year,
                    EngineDisplacement = cccvm.cccv.ccc.car.EngineDisplacement,
                    FuelType = cccvm.cccv.ccc.car.FuelType,
                    Mileage = cccvm.cccv.ccc.customerCar.Km,
                    CarName = cccvm.cccv.ccc.customerCar.Name,
                    CV = cccvm.cccv.ccc.car.CV,
                    CustomerId = cccvm.cccv.ccc.customerCar.CustomerId
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

            if (!string.IsNullOrWhiteSpace(query.Make) && !string.IsNullOrWhiteSpace(query.Model) && query.Year != null)
            {
                var carMake = await _dbContext.CarMakes
                    .FirstOrDefaultAsync(m => m.Name == query.Make)
                    ?? throw new Exception("Car make not found.");

                var carModel = await _dbContext.CarModels
                    .FirstOrDefaultAsync(m => m.Name == query.Model && m.CarMakeId == carMake.Id)
                    ?? throw new Exception("Car model not found.");

                var carVersion = await _dbContext.CarModelVersions
                  .FirstOrDefaultAsync(car => car.CarModelId == carModel.Id && car.Year == query.Year)
                  ?? throw new Exception("Car model not found.");

                car.CarModelVersionId = carVersion.Id;
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
