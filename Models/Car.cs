using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicDrive.Enums;

namespace TicDrive.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]{2}[0-9]{3}[A-Z]{2}$", ErrorMessage = "Licence plate must be in the format XX999XX.")]
        public string LicencePlate { get; set; }

        [Required]
        public int CarModelVersionId { get; set; }

        [ForeignKey(nameof(CarModelVersionId))]
        public CarModelVersion? CarModelVersion { get; set; }
        public FuelType? FuelType { get; set; }
        public TransmissionType? TransmissionType { get; set; }
       public string? EngineDisplacement { get; set; }
        public int? CV { get; set; }
    }
}
