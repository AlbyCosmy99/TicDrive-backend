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

        Task<List<FullBookingDto>> GetBookingsAsync(string userId, UserType userType);
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

        public async Task<List<FullBookingDto>> GetBookingsAsync(string userId, UserType userType)
        {
            var query = _context.Bookings
                .Include(q => q.Customer)
                .Include(q => q.Workshop)
                .Include(q => q.Service)
                .Include(q => q.CustomerCar);

            var imagesDict = new Dictionary<string, List<FullUserImageDto>>();

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
                    imagesDict[customerId] = _mapper.Map<List<FullUserImageDto>>(images);
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
                .Select(j => new FullBookingDto
                {
                    Id = j.qw.q.Id,
                    BookingDate = j.qw.q.BookingDate,
                    AppointmentDate = j.qw.q.AppointmentDate,

                    CustomerId = j.qw.q.CustomerId,
                    CustomerName = j.qw.q.Customer.Name,
                    CustomerImage = userType == UserType.Workshop &&
                                    imagesDict.ContainsKey(j.qw.q.CustomerId) &&
                                    imagesDict[j.qw.q.CustomerId].Count > 0
                                    ? imagesDict[j.qw.q.CustomerId][0]
                                    : null,

                    WorkshopId = j.qw.q.WorkshopId,
                    WorkshopAddress = j.qw.q.Workshop.Address,
                    WorkshopName = j.qw.workshopDetails.WorkshopName,

                    FinalPrice = j.qw.q.FinalPrice,
                    Status = j.qw.q.Status,

                    ServiceId = j.qw.q.ServiceId,
                    ServiceName = j.qw.q.Service.Key,

                    CustomerCarId = j.qw.q.CustomerCarId,
                    CustomerCarName = j.qw.q.CustomerCar.Name,
                    CustomerCarPlate = j.car.LicencePlate,
                    CustomerCarYear = 1990,
                    CustomerCarLogoUrl = ""
                })
                .ToList();

            return result;
        }
    }
}
