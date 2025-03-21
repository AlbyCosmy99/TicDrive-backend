using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicDrive.Enums;

namespace TicDrive.Models
{
    public class Car : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Year is not null)
            {
                int currentYear = DateTime.Now.Year;
                if (Year < 1700 || Year > currentYear)
                {
                    yield return new ValidationResult(
                        $"Year must be between 1700 and {currentYear}.",
                        [nameof(Year)]
                    );
                }
            }
        }

        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]{2}[0-9]{3}[A-Z]{2}$", ErrorMessage = "Licence plate must be in the format XX999XX.")]
        public string? LicencePlate { get; set; }

        [Required]
        public int CarModelId { get; set; }

        [ForeignKey(nameof(CarModelId))]
        public CarModel? CarModel { get; set; }
        public int? Year { get; set; }
        public FuelType? FuelType { get; set; }
        public TransmissionType? TransmissionType { get; set; }
       public string? EngineDisplacement { get; set; }
    }
}
