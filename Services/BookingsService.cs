using TicDrive.Context;
using TicDrive.Models;
using TicDrive.Enums;
using Microsoft.EntityFrameworkCore;
using TicDrive.Dto.BookingDto;
using TicDrive.Dto.UserImageDto;
using AutoMapper;

namespace TicDrive.Services
{
    public interface IBookingsService
    {
        Task<(bool Success, string Message, int? BookingId)> BookAServiceAsync(
            string customerId,
            UserType userType,
            string workshopId,
            int serviceId,
            int customerCarId,
            DateTime appointmentDate,
            decimal finalPrice
        );

        Task<List<FullBookingDto>> GetBookingsAsync(string userId, UserType userType, string languageCode = "it");
    }

    public class BookingsService : IBookingsService
    {
        private readonly TicDriveDbContext _context;
        private readonly IImagesService _imagesService;
        private readonly IMapper _mapper;

        public BookingsService(TicDriveDbContext context, IImagesService imagesService, IMapper mapper)
        {
            _context = context;
            _imagesService = imagesService;
            _mapper = mapper;
        }

        public async Task<(bool Success, string Message, int? BookingId)> BookAServiceAsync(
            string customerId,
            UserType userType,
            string workshopId,
            int serviceId,
            int customerCarId,
            DateTime appointmentDate,
            decimal finalPrice
        )
        {
            if (string.IsNullOrEmpty(customerId) || userType != UserType.Customer)
                return (false, "Invalid customer", null);

            var workshop = await _context.Users.FindAsync(workshopId);
            if (workshop == null || workshop.UserType != UserType.Workshop)
                return (false, "Invalid workshop", null);

            var service = await _context.Services.FindAsync(serviceId);
            if (service == null)
                return (false, "Invalid service", null);

            var customerCar = await _context.CustomerCars
                .FirstOrDefaultAsync(cc => cc.Id == customerCarId && cc.CustomerId == customerId);

            if (customerCar == null)
                return (false, "This car does not belong to the current customer", null);

            var serviceOffered = await _context.OfferedServices
                .AnyAsync(os => os.WorkshopId == workshopId && os.ServiceId == serviceId);

            if (!serviceOffered)
                return (false, "Selected workshop does not offer the chosen service", null);

            var booking = new Booking
            {
                CustomerId = customerId,
                WorkshopId = workshopId,
                ServiceId = serviceId,
                CustomerCarId = customerCarId,
                AppointmentDate = appointmentDate,
                BookingDate = DateTime.UtcNow,
                FinalPrice = finalPrice,
                Status = BookingType.Waiting
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return (true, "Booking created", booking.Id);
        }

        public async Task<List<FullBookingDto>> GetBookingsAsync(string userId, UserType userType, string languageCode = "it")
        {
            var query = _context.Bookings
                .Include(q => q.Customer)
                .Include(q => q.Workshop)
                .Include(q => q.Service)
                .Include(q => q.CustomerCar);

            var customersImagesDict = new Dictionary<string, List<FullUserImageDto>>();
            var workshopsImagesDict = new Dictionary<string, List<FullUserImageDto>>();

            if (userType == UserType.Workshop)
            {
                var uniqueCustomers = await query
                    .Where(booking => booking.WorkshopId == userId)
                    .Select(booking => booking.CustomerId)
                    .Distinct()
                    .ToListAsync();

                foreach (var customerId in uniqueCustomers)
                {
                    var images = await _imagesService.GetUserImagesAsync(customerId, 1);
                    customersImagesDict[customerId] = _mapper.Map<List<FullUserImageDto>>(images);
                }
            }

            if (userType == UserType.Customer)
            {
                var uniqueWorkshops = await query
                    .Where(booking => booking.CustomerId == userId)
                    .Select(booking => booking.WorkshopId)
                    .Distinct()
                    .ToListAsync();

                foreach (var workshopId in uniqueWorkshops)
                {
                    var images = await _imagesService.GetUserImagesAsync(workshopId, 1);
                    workshopsImagesDict[workshopId] = _mapper.Map<List<FullUserImageDto>>(images);
                }
            }

            var result = query
                .Join(_context.WorkshopsDetails,
                    q => q.WorkshopId,
                    workshopDetails => workshopDetails.WorkshopId,
                    (q, workshopDetails) => new { q, workshopDetails })
                .Join(_context.Cars,
                    qw => qw.q.CustomerCar.CarId,
                    car => car.Id,
                    (qw, car) => new { qw, car })
                 .Join(_context.CarModelVersions,
                    qwCar => qwCar.car.CarModelVersionId,
                    carModelVersion => carModelVersion.Id,
                    (qwCar, carModelVersion) => new { qwCar, carModelVersion })
                 .Join(_context.CarModels,
                    qwCarmv => qwCarmv.carModelVersion.CarModelId,
                    model => model.Id,
                    (qwCarmv, model) => new { qwCarmv, model })
                 .Join(_context.CarMakes,
                      qwCarmvModel => qwCarmvModel.model.CarMakeId,
                      make => make.Id,
                      (qwCarmvModel, make) => new { qwCarmvModel, make })
                .Join(_context.ServicesTranslations
                    .Where(translation => translation.LanguageId == (languageCode == "en" ? 1 : 2)),
                qwCarmvModelMake => qwCarmvModelMake.qwCarmvModel.qwCarmv.qwCar.qw.q.ServiceId,
                translation => translation.ServiceId,
                (qwCarmvModelMake, carTranslations) => new { qwCarmvModelMake, carTranslations })    
                .Select(j => new FullBookingDto
                {
                    Id = j.qwCarmvModelMake.qwCarmvModel.qwCarmv.qwCar.qw.q.Id,
                    BookingDate = j.qwCarmvModelMake.qwCarmvModel.qwCarmv.qwCar.qw.q.BookingDate,
                    AppointmentDate = j.qwCarmvModelMake.qwCarmvModel.qwCarmv.qwCar.qw.q.AppointmentDate,

                    CustomerId = j.qwCarmvModelMake.qwCarmvModel.qwCarmv.qwCar.qw.q.CustomerId,
                    CustomerName = j.qwCarmvModelMake.qwCarmvModel.qwCarmv.qwCar.qw.q.Customer.Name,
                    CustomerImage = userType == UserType.Workshop &&
                                    customersImagesDict.ContainsKey(j.qwCarmvModelMake.qwCarmvModel.qwCarmv.qwCar.qw.q.CustomerId) &&
                                    customersImagesDict[j.qwCarmvModelMake.qwCarmvModel.qwCarmv.qwCar.qw.q.CustomerId].Count > 0
                                    ? customersImagesDict[j.qwCarmvModelMake.qwCarmvModel.qwCarmv.qwCar.qw.q.CustomerId][0]
                                    : null,

                    WorkshopId = j.qwCarmvModelMake.qwCarmvModel.qwCarmv.qwCar.qw.q.WorkshopId,
                    WorkshopAddress = j.qwCarmvModelMake.qwCarmvModel.qwCarmv.qwCar.qw.q.Workshop.Address,
                    WorkshopName = j.qwCarmvModelMake.qwCarmvModel.qwCarmv.qwCar.qw.workshopDetails.WorkshopName,
                    WorkshopImage = userType == UserType.Customer &&
                                    workshopsImagesDict.ContainsKey(j.qwCarmvModelMake.qwCarmvModel.qwCarmv.qwCar.qw.q.WorkshopId) &&
                                    workshopsImagesDict[j.qwCarmvModelMake.qwCarmvModel.qwCarmv.qwCar.qw.q.WorkshopId].Count > 0
                                    ? workshopsImagesDict[j.qwCarmvModelMake.qwCarmvModel.qwCarmv.qwCar.qw.q.WorkshopId][0]
                                    : null,

                    FinalPrice = j.qwCarmvModelMake.qwCarmvModel.qwCarmv.qwCar.qw.q.FinalPrice,
                    Status = j.qwCarmvModelMake.qwCarmvModel.qwCarmv.qwCar.qw.q.Status,

                    ServiceId = j.qwCarmvModelMake.qwCarmvModel.qwCarmv.qwCar.qw.q.ServiceId,
                    ServiceName = j.carTranslations.Title,

                    CustomerCarId = j.qwCarmvModelMake.qwCarmvModel.qwCarmv.qwCar.qw.q.CustomerCarId,
                    CustomerCarName = j.qwCarmvModelMake.qwCarmvModel.qwCarmv.qwCar.qw.q.CustomerCar.Name,
                    CustomerCarPlate = j.qwCarmvModelMake.qwCarmvModel.qwCarmv.qwCar.car.LicencePlate,
                    CustomerCarMake = j.qwCarmvModelMake.make.Name,
                    CustomerCarModel = j.qwCarmvModelMake.qwCarmvModel.model.Name,
                    CustomerCarYear = j.qwCarmvModelMake.qwCarmvModel.qwCarmv.carModelVersion.Year,
                    CustomerCarLogoUrl = j.qwCarmvModelMake.make.LogoUrl
                })
                .ToList();

            return result;
        }
    }
}
