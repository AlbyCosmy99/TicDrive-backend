using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TicDrive.Context;
using TicDrive.Enums;
using TicDrive.Services;
using System.Globalization;

namespace TicDrive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IBookingsService _bookingsService;
        private readonly IEmailService _emailService;
        private readonly TicDriveDbContext _context;

        public BookingsController(
            IAuthService authService,
            IBookingsService bookingsService,
            IEmailService emailService,
            TicDriveDbContext context)
        {
            _authService = authService;
            _bookingsService = bookingsService;
            _emailService = emailService;
            _context = context;
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

            if (userId == null || userType != UserType.Customer)
                return Unauthorized("Non sei autorizzato a prenotare un servizio.");

            var result = await _bookingsService.BookAServiceAsync(
                userId,
                (UserType)userType,
                payload.WorkshopId,
                payload.ServiceId,
                payload.CustomerCarId,
                payload.AppointmentDate,
                payload.FinalPrice
            );

            if (!result.Success)
                return BadRequest(result.Message);

            var customer = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var workshop = await _context.Users.FirstOrDefaultAsync(u => u.Id == payload.WorkshopId);
            var workshopDetails = await _context.WorkshopsDetails.FirstOrDefaultAsync(workshop => workshop.WorkshopId == payload.WorkshopId);
            var service = await _context.ServicesTranslations.FirstOrDefaultAsync(s => s.ServiceId == payload.ServiceId && s.LanguageId == 2);

            if (customer?.EmailConfirmed == true && !string.IsNullOrEmpty(customer.Email))
            {
                var italianCulture = new CultureInfo("it-IT");
                TextInfo textInfo = italianCulture.TextInfo;

                var formattedDate = textInfo.ToTitleCase(
                    payload.AppointmentDate.ToString("dddd dd MMMM yyyy - HH:mm", italianCulture)
                );

                var emailBody = $@"
                    <p>Ciao {customer.Name},</p>
                    <p>Hai prenotato con successo il servizio <strong>{service?.Title}</strong> presso l'officina <strong>{workshopDetails?.WorkshopName}</strong>.</p>
                    <p><strong>Data appuntamento:</strong> {formattedDate}</p>
                    <p><strong>Prezzo:</strong> €{payload.FinalPrice:F2}</p>
                    <p>Grazie per aver scelto TicDrive!</p>";

                await _emailService.SendEmailAsync(
                    customer.Email,
                    "Conferma prenotazione TicDrive",
                    emailBody
                );
            }

            //if (workshop?.EmailConfirmed == true && !string.IsNullOrEmpty(workshop.Email))
            //{
            //    var italianCulture = new CultureInfo("it-IT");
            //    var formattedDate = payload.AppointmentDate.ToString("dddd dd MMMM yyyy - HH:mm", italianCulture);

            //    var emailBody = $@"
            //    <p>Ciao {workshop.Name},</p>
            //    <p>Hai ricevuto una nuova prenotazione per il servizio <strong>{service?.Title}</strong> da parte del cliente <strong>{customer?.Name}</strong>.</p>
            //    <p><strong>Data appuntamento:</strong> {formattedDate}</p>
            //    <p><strong>Prezzo concordato:</strong> €{payload.FinalPrice:F2}</p>
            //    <p>Accedi alla tua dashboard per maggiori dettagli.</p>";

            //    await _emailService.SendEmailAsync(
            //        workshop.Email,
            //        "Nuova prenotazione su TicDrive",
            //        emailBody
            //    );
            //}

            return Ok(new { result.Message, result.BookingId });
        }
    }
}
