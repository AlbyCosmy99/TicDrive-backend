using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicDrive.Models
{
    public class CarModelVersion : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
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
        public int Id { get; set; }
        public int CarModelId { get; set; }
        [ForeignKey(nameof(CarModelId))]
        public virtual CarModel? CarModel{ get; set; }
        public int Year { get; set; }
    }
}
