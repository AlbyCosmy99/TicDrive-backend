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
        Task<(bool, int)> PostCustomerCar(AddCarQuery query, string customerId);
        Task UpdateCustomerCar(AddCarQuery query, string customerId);
        List<FullCustomerCarDto> GetCustomerCars(string customerId);
        Task<bool> DeleteCustomerCar(int carId);
    }

    public class CarsService : ICarsService
    {
        private readonly TicDriveDbContext _dbContext;

        public CarsService(TicDriveDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<CarMake> GetMakes() => _dbContext.CarMakes.ToList();

        public List<CarModel> GetCarModelsByMakeId(int makeId) =>
            _dbContext.CarModels.Where(model => model.CarMakeId == makeId).ToList();

        public List<CarModelVersion> GetModelVersions(int modelId) =>
            _dbContext.CarModelVersions.Where(version => version.CarModelId == modelId).ToList();

        public async Task<(bool, int)> PostCustomerCar(AddCarQuery query, string customerId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            var car = await _dbContext.Cars.FirstOrDefaultAsync(c => c.LicencePlate == query.Plate);

            if (car == null)
            {
                var carMake = await _dbContext.CarMakes.FirstOrDefaultAsync(m => m.Name == query.Make)
                    ?? throw new Exception("Car make not found.");

                var carModel = await _dbContext.CarModels.FirstOrDefaultAsync(m => m.Name == query.Model && m.CarMakeId == carMake.Id)
                    ?? throw new Exception("Car model not found.");

                var carVersion = await _dbContext.CarModelVersions.FirstOrDefaultAsync(cv => cv.Year == query.Year && cv.CarModelId == carModel.Id)
                    ?? throw new Exception("Car version not found.");

                car = new Car
                {
                    LicencePlate = query.Plate!,
                    CarModelVersionId = carVersion.Id,
                    FuelType = query.FuelType,
                    TransmissionType = query.TransmissionType,
                    EngineDisplacement = query.EngineDisplacement,
                    CV = query.CV
                };

                _dbContext.Cars.Add(car);
                await _dbContext.SaveChangesAsync();
            }

            var customerCar = await _dbContext.CustomerCars.FirstOrDefaultAsync(c => c.CustomerId == customerId && c.CarId == car.Id);
            if (customerCar != null)
            {
                return (false, customerCar.Id); // already registered
            }

            var newCustomerCar = new CustomerCar
            {
                CarId = car.Id,
                CustomerId = customerId,
                Name = query.Name ?? "My Car",
                Km = query.Mileage
            };

            _dbContext.CustomerCars.Add(newCustomerCar);
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
            return (true, newCustomerCar.Id);
        }

        public async Task UpdateCustomerCar(AddCarQuery query, string customerId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            var customerCar = await _dbContext.CustomerCars
                .FirstOrDefaultAsync(cc => cc.CustomerId == customerId && cc.CarId == query.Id)
                ?? throw new Exception("Car not found.");

            if (query.Mileage != null)
                customerCar.Km = query.Mileage;

            if (!string.IsNullOrWhiteSpace(query.Name))
                customerCar.Name = query.Name;

            var car = await _dbContext.Cars.FirstOrDefaultAsync(c => c.Id == customerCar.CarId)
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
                var carMake = await _dbContext.CarMakes.FirstOrDefaultAsync(m => m.Name == query.Make)
                    ?? throw new Exception("Car make not found.");

                var carModel = await _dbContext.CarModels.FirstOrDefaultAsync(m => m.Name == query.Model && m.CarMakeId == carMake.Id)
                    ?? throw new Exception("Car model not found.");

                var carVersion = await _dbContext.CarModelVersions.FirstOrDefaultAsync(cv => cv.CarModelId == carModel.Id && cv.Year == query.Year)
                    ?? throw new Exception("Car version not found.");

                car.CarModelVersionId = carVersion.Id;
            }

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public List<FullCustomerCarDto> GetCustomerCars(string customerId)
        {
            return _dbContext.CustomerCars
                .Where(customerCar => customerCar.CustomerId == customerId)
                .Join(_dbContext.Cars,
                    cc => cc.CarId,
                    c => c.Id,
                    (cc, c) => new { cc, c })
                .Join(_dbContext.CarModelVersions,
                    temp => temp.c.CarModelVersionId,
                    mv => mv.Id,
                    (temp, mv) => new { temp, mv })
                .Join(_dbContext.CarModels,
                    temp2 => temp2.mv.CarModelId,
                    cm => cm.Id,
                    (temp2, cm) => new { temp2, cm })
                .Join(_dbContext.CarMakes,
                    temp3 => temp3.cm.CarMakeId,
                    mk => mk.Id,
                    (temp3, mk) => new FullCustomerCarDto
                    {
                        Id = temp3.temp2.temp.cc.Id,
                        Make = mk.Name,
                        Model = temp3.cm.Name,
                        PlateNumber = temp3.temp2.temp.c.LicencePlate!,
                        Year = temp3.temp2.mv.Year,
                        EngineDisplacement = temp3.temp2.temp.c.EngineDisplacement,
                        FuelType = temp3.temp2.temp.c.FuelType,
                        Mileage = temp3.temp2.temp.cc.Km,
                        CarName = temp3.temp2.temp.cc.Name,
                        CV = temp3.temp2.temp.c.CV,
                        CustomerId = temp3.temp2.temp.cc.CustomerId,
                        LogoUrl = mk.LogoUrl
                    })
                .ToList();
        }

        public async Task<bool> DeleteCustomerCar(int carId)
        {
            var car = await _dbContext.CustomerCars.FirstOrDefaultAsync(c => c.Id == carId);
            if (car == null) return false;

            _dbContext.CustomerCars.Remove(car);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
