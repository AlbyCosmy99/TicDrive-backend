using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicDrive.Models
{
    public class CarModel : IValidatableObject
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
        public required string Name { get; set; }
        public int Year { get; set; }
        [Required]
        public int CarMakeId { get; set; }
        [ForeignKey(nameof(CarMakeId))]
        public virtual CarMake? CarMake { get; set; }
    }
}
