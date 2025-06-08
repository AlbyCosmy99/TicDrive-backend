using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TicDrive.Context;
using TicDrive.Enums;
using TicDrive.Services;
using System.Globalization;
using System.Threading.Tasks;
using TicDrive.Models;

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
        private readonly IServicesService _servicesService;

        public BookingsController(
            IAuthService authService,
            IBookingsService bookingsService,
            IEmailService emailService,
            TicDriveDbContext context,
            IServicesService servicesService)
        {
            _authService = authService;
            _bookingsService = bookingsService;
            _emailService = emailService;
            _context = context;
            _servicesService = servicesService;
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
            var workshopDetails = await _context.WorkshopsDetails.FirstOrDefaultAsync(w => w.WorkshopId == payload.WorkshopId);
            var service = await _context.ServicesTranslations.FirstOrDefaultAsync(s => s.ServiceId == payload.ServiceId && s.LanguageId == 2);

            var carInfo = await _context.CustomerCars
                .Where(cc => cc.Id == payload.CustomerCarId)
                .Join(_context.Cars,
                    cc => cc.CarId,
                    car => car.Id,
                    (cc, car) => new { cc, car })
                .Join(_context.CarModelVersions,
                    ccCar => ccCar.car.CarModelVersionId,
                    version => version.Id,
                    (ccCar, version) => new { ccCar.cc, ccCar.car, version })
                .Join(_context.CarModels,
                    full => full.version.CarModelId,
                    model => model.Id,
                    (full, model) => new { full.cc, full.car, full.version, model })
                .Join(_context.CarMakes,
                    full => full.model.CarMakeId,
                    make => make.Id,
                    (full, make) => new
                    {
                        Make = make.Name,
                        Model = full.model.Name,
                        full.version.Year,
                        Plate = full.car.LicencePlate
                    })
                .Select(car => $"{car.Make} {car.Model} - {car.Year} - {car.Plate}")
                .FirstOrDefaultAsync();



            var italianCulture = new CultureInfo("it-IT");
            var formattedDate = italianCulture.TextInfo.ToTitleCase(
                payload.AppointmentDate.ToString("dddd dd MMMM yyyy - HH:mm", italianCulture)
            );

            // EMAIL AL CLIENTE
            if (customer?.EmailConfirmed == true && !string.IsNullOrEmpty(customer.Email))
            {
                var serviceName = await _servicesService.GetFullServiceName(service.Id, "it");

                var emailBody = $@"
                    <p>Ciao {customer.Name},</p>
                    <p>Hai prenotato con successo il servizio <strong>{serviceName}</strong> presso l'officina <strong>{workshopDetails?.WorkshopName}</strong>.</p>
                    <p><strong>Telefono officina:</strong> {workshop.PhoneNumber}</p>
                    <p><strong>Auto:</strong> {carInfo}</p>
                    <p><strong>Data appuntamento:</strong> {formattedDate}</p>
                    <p><strong>Prezzo:</strong> €{payload.FinalPrice:F2}</p>
                    <p><strong>Codice PIN:</strong> {result.booking?.PinCode} <em>(Da presentare in officina)</em></p>
                    <p>Grazie per aver scelto TicDrive!</p>";

                await _emailService.SendEmailAsync(
                    customer.Email,
                    "Conferma prenotazione TicDrive",
                    emailBody
                );
            }

            //// EMAIL ALL'OFFICINA
            //if (workshop?.EmailConfirmed == true && !string.IsNullOrEmpty(workshop.Email))
            //{
            //    var serviceName = await _servicesService.GetFullServiceName(service.Id, "it");

            //    var emailBody = $@"
            //        <p>Ciao {workshop.Name},</p>
            //        <p>Hai ricevuto una nuova prenotazione per il servizio <strong>{serviceName}</strong> da parte del cliente <strong>{customer?.Name}</strong>.</p>
            //        <p><strong>Email cliente:</strong> {customer?.Email}</p>
            //        <p><strong>Auto:</strong> {carInfo}</p>
            //        <p><strong>Data appuntamento:</strong> {formattedDate}</p>
            //        <p><strong>Prezzo concordato:</strong> €{payload.FinalPrice:F2}</p>
            //        <p><strong>Codice PIN:</strong> {result.booking?.PinCode} <em>(Il cliente deve presentarlo in officina per identificare la prenotazione)</em></p>
            //        <p>Accedi alla tua dashboard su <strong>ticdrive.it</strong> per maggiori dettagli.</p>";

            //    await _emailService.SendEmailAsync(
            //        workshop.Email,
            //        "Nuova prenotazione su TicDrive",
            //        emailBody
            //    );
            //}

            return Ok(new { result.Message, bookingId = result.booking.Id, bookingPinCode = result.booking.PinCode });
        }

        public class BookingsQuery
        {
            public string? BookingType { get; set; }
            public int Skip { get; set; } = 0;
            public int Take { get; set; } = 10;

            public BookingType? GetParsedBookingType()
            {
                if (Enum.TryParse<BookingType>(BookingType, true, out var parsed))
                    return parsed;
                return null;
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetBookings([FromQuery] BookingsQuery query)
        {
            var userClaims = _authService.GetUserClaims(this);
            var userId = _authService.GetUserId(userClaims);
            var userType = _authService.GetUserType(userClaims);

            if (userId == null || userType == null)
                return Unauthorized("Non sei autorizzato ad accedere alle prenotazioni.");
            var parsedBookingType = query.GetParsedBookingType();
            var bookings = await _bookingsService.GetBookingsAsync(userId, (UserType)userType, parsedBookingType);
            return Ok(new { bookings.Count, Bookings = bookings.Skip(query.Skip).Take(query.Take) });
        }
    }
}
