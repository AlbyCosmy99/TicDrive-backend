using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicDrive.Models
{
    public class UserImage
    {
        public int Id { get; set; }

        [Required]
        public required string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Required]
        public required string Filename { get; set; }
        public bool? IsMainImage { get; set; } = false;
    }
}
