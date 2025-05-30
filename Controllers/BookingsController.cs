using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TicDrive.Services;

namespace TicDrive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IBookingsService _bookingsService;

        public BookingsController(IAuthService authService, IBookingsService bookingsService)
        {
            _authService = authService;
            _bookingsService = bookingsService;
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

            if (userId == null || userType == null)
            {
                return Unauthorized("User is not authorize to book a service.");
            }

            var result = await _bookingsService.BookAServiceAsync(
                userId,
                (Enums.UserType) userType,
                payload.WorkshopId,
                payload.ServiceId,
                payload.CustomerCarId,
                payload.AppointmentDate,
                payload.FinalPrice
            );

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { result.Message, result.BookingId });
        }

    }
}
