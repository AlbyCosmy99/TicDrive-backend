using TicDrive.Context;
using TicDrive.Models;
using TicDrive.Enums;
using Microsoft.EntityFrameworkCore;

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
    }

    public class BookingsService : IBookingsService
    {
        private readonly TicDriveDbContext _context;

        public BookingsService(TicDriveDbContext context)
        {
            _context = context;
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
                .FirstOrDefaultAsync(cc => cc.Id == customerCarId && cc.CustomerId== customerId);

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
    }
}
