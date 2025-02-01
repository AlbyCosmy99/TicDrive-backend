using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicDrive.Models
{
    public class CarModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int Year { get; set; }
        [Required]
        public int CarMakeId { get; set; }
        [ForeignKey(nameof(CarMakeId))]
        public virtual CarMake? CarMake { get; set; }
    }
}
