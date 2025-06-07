using TicDrive.Context;
using TicDrive.Models;
using TicDrive.Enums;
using Microsoft.EntityFrameworkCore;
using TicDrive.Dto.BookingDto;
using TicDrive.Dto.UserImageDto;
using AutoMapper;
using System.Globalization;

namespace TicDrive.Services
{
    public interface IBookingsService
    {
        Task<(bool Success, string Message, Booking booking)> BookAServiceAsync(
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
        private readonly IServicesService _servicesService;

        public BookingsService(TicDriveDbContext context, IImagesService imagesService, IMapper mapper, IServicesService servicesService)
        {
            _context = context;
            _imagesService = imagesService;
            _mapper = mapper;
            _servicesService = servicesService;
        }

        private async Task<string> GenerateUniquePinCodeAsync()
        {
            var random = new Random();
            string pinCode;

            do
            {
                pinCode = random.Next(0, 1000000).ToString("D6");
            }
            while (await _context.Bookings.AnyAsync(b => b.PinCode == pinCode));

            return pinCode;
        }

        public async Task<(bool Success, string Message, Booking booking)> BookAServiceAsync(
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

            var pinCode = await GenerateUniquePinCodeAsync();

            var booking = new Booking
            {
                CustomerId = customerId,
                WorkshopId = workshopId,
                ServiceId = serviceId,
                CustomerCarId = customerCarId,
                AppointmentDate = appointmentDate,
                BookingDate = DateTime.UtcNow,
                FinalPrice = finalPrice,
                Status = BookingType.Waiting,
                PinCode = pinCode
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return (true, "Booking created", booking);
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

            var rawBookings = await query
                .Join(_context.WorkshopsDetails,
                    b => b.WorkshopId,
                    wd => wd.WorkshopId,
                    (b, wd) => new { b, wd })
                .Join(_context.Cars,
                    bw => bw.b.CustomerCar.CarId,
                    c => c.Id,
                    (bw, c) => new { bw, c })
                .Join(_context.CarModelVersions,
                    bwc => bwc.c.CarModelVersionId,
                    cmv => cmv.Id,
                    (bwc, cmv) => new { bwc, cmv })
                .Join(_context.CarModels,
                    bwccmv => bwccmv.cmv.CarModelId,
                    cm => cm.Id,
                    (bwccmv, cm) => new { bwccmv, cm })
                .Join(_context.CarMakes,
                    full => full.cm.CarMakeId,
                    make => make.Id,
                    (full, make) => new
                    {
                        Booking = full.bwccmv.bwc.bw.b,
                        Customer = full.bwccmv.bwc.bw.b.Customer,
                        Workshop = full.bwccmv.bwc.bw.b.Workshop,
                        WorkshopDetails = full.bwccmv.bwc.bw.wd,
                        Car = full.bwccmv.bwc.c,
                        CarModelVersion = full.bwccmv.cmv,
                        CarModel = full.cm,
                        CarMake = make
                    })
                .ToListAsync();

            var result = new List<FullBookingDto>();

            foreach (var item in rawBookings)
            {
                if (userType == UserType.Workshop && item.Booking.WorkshopId != userId)
                    continue;

                if (userType == UserType.Customer && item.Booking.CustomerId != userId)
                    continue;

                var serviceName = await _servicesService.GetFullServiceName(item.Booking.ServiceId, languageCode);

                result.Add(new FullBookingDto
                {
                    Id = item.Booking.Id,
                    BookingDate = item.Booking.BookingDate.ToString("dddd dd MMMM yyyy - HH:mm", new CultureInfo("it-IT")),
                    AppointmentDate = item.Booking.AppointmentDate.ToString("dddd dd MMMM yyyy - HH:mm", new CultureInfo("it-IT")),

                    CustomerId = item.Booking.CustomerId,
                    CustomerName = item.Customer.Name,
                    CustomerImage = userType == UserType.Workshop &&
                                    customersImagesDict.ContainsKey(item.Booking.CustomerId) &&
                                    customersImagesDict[item.Booking.CustomerId].Count > 0
                                    ? customersImagesDict[item.Booking.CustomerId][0]
                                    : null,

                    WorkshopId = item.Booking.WorkshopId,
                    WorkshopAddress = item.Workshop.Address,
                    WorkshopName = item.WorkshopDetails.WorkshopName,
                    WorkshopImage = userType == UserType.Customer &&
                                    workshopsImagesDict.ContainsKey(item.Booking.WorkshopId) &&
                                    workshopsImagesDict[item.Booking.WorkshopId].Count > 0
                                    ? workshopsImagesDict[item.Booking.WorkshopId][0]
                                    : null,

                    FinalPrice = item.Booking.FinalPrice,
                    Status = item.Booking.Status,

                    ServiceId = item.Booking.ServiceId,
                    ServiceName = serviceName,

                    CustomerCarId = item.Booking.CustomerCarId,
                    CustomerCarName = item.Booking.CustomerCar.Name,
                    CustomerCarPlate = item.Car.LicencePlate,
                    CustomerCarMake = item.CarMake.Name,
                    CustomerCarModel = item.CarModel.Name,
                    CustomerCarYear = item.CarModelVersion.Year,
                    CustomerCarLogoUrl = item.CarMake.LogoUrl,
                    PinCode = item.Booking.PinCode
                });
            }

            return result;
        }
    }
}
