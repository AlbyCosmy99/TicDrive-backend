using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicDrive.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public required string LicencePlate { get; set; }
        [Required]
        public int CarModelId { get; set; }
        [Required]
        public string? OwnerId { get; set; }
        [ForeignKey(nameof(CarModelId))]
        public required CarModel CarModel { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public required User Owner { get; set; }
    }
}
