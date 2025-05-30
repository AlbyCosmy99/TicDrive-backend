using Microsoft.AspNetCore.Mvc;
using TicDrive.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicDrive.Context;
using Microsoft.AspNetCore.Authorization;
using TicDrive.Services;

namespace TicDrive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly TicDriveDbContext _context;
        private readonly IAuthService _authService;

        public BookingsController(TicDriveDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public class BookServiceBody
        {
            public string WorkshopId { get; set; }
            public int ServiceId { get; set; }
            public int CustomerCarId { get; set; }
            public DateTime AppointmentDate { get; set; }
            public decimal FinalPrice { get; set; }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> BookService([FromBody] BookServiceBody payload)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userClaims = _authService.GetUserClaims(this);
            var userId = _authService.GetUserId(userClaims);
            var userType = _authService.GetUserType(userClaims);

            var workshop = await _context.Users.FindAsync(payload.WorkshopId);
            var service = await _context.Services.FindAsync(payload.ServiceId);
            var customerCar = await _context.CustomerCars.FindAsync(payload.CustomerCarId);

            if (userId == null || userType != Enums.UserType.Customer)
                return BadRequest("Invalid customer");

            if (workshop == null || workshop.UserType != Enums.UserType.Workshop)
                return BadRequest("Invalid workshop");

            if (service == null)
                return BadRequest("Invalid service");

            if (customerCar == null)
                return BadRequest("Invalid car");

            var booking = new Booking
            {
                CustomerId = userId,
                WorkshopId = payload.WorkshopId,
                ServiceId = payload.ServiceId,
                CustomerCarId = payload.CustomerCarId,
                AppointmentDate = payload.AppointmentDate,
                BookingDate = DateTime.UtcNow,
                FinalPrice = payload.FinalPrice,
                Status = BookingType.Waiting
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Booking created", BookingId = booking.Id });
        }
    }
}
