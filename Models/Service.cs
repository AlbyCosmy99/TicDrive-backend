using System.ComponentModel.DataAnnotations.Schema;

namespace TicDrive.Models
{
    public class Service
    {
        public int Id { get; set; }

        public required string Key { get; set; }

        public string? Icon { get; set; }

        public string? Bg_Image { get; set; }

        public int? FatherId { get; set; } = null;

        [ForeignKey(nameof(FatherId))]
        public Service? Father { get; set; }

    }
}
