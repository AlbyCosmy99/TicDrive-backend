using System.Text.Json.Serialization;
using TicDrive.Dto.UserImageDto;

namespace TicDrive.Dto.BookingDto
{
    public class FullBookingDto
    {
        public int Id { get; set; }
        public required string BookingDate { get; set; }
        public required string AppointmentDate { get; set; }

        public required string CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public FullUserImageDto? CustomerImage { get; set; }

        public required string WorkshopId { get; set; }
        public string? WorkshopName { get; set; }
        public string? WorkshopAddress { get; set; }
        public FullUserImageDto? WorkshopImage { get; set; }

        public decimal FinalPrice { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BookingType Status { get; set; }

        public int ServiceId { get; set; }
        public string? ServiceName { get; set; }

        public int CustomerCarId { get; set; }
        public string? CustomerCarName { get; set; }
        public string? CustomerCarPlate { get; set; }
        public required string CustomerCarMake { get; set; }
        public required string CustomerCarModel { get; set; }
        public int CustomerCarYear { get; set; }
        public string? CustomerCarLogoUrl { get; set; }
    }
}
