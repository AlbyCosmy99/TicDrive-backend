using System.ComponentModel.DataAnnotations.Schema;
using TicDrive.Models.DateTime;

namespace TicDrive.Models
{
    public class DayTranslation
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        [ForeignKey(nameof(LanguageId))]
        public Language Language { get; set; }
        public int DayId { get; set; }
        [ForeignKey(nameof(DayId))]
        public Day Day { get; set; }
        public required string Label { get; set; }
    }
}
