using System.ComponentModel.DataAnnotations.Schema;

namespace TicDrive.Models
{
    public class ServiceTranslation
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }

        [ForeignKey(nameof(LanguageId))]
        public Language Language { get; set; } = null!;

        public int ServiceId { get; set; }

        [ForeignKey(nameof(ServiceId))]
        public Service Service { get; set; } = null!;

        public required string Title { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
