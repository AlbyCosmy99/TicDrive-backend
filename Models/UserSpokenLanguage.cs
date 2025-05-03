using System.ComponentModel.DataAnnotations.Schema;

namespace TicDrive.Models
{
    public class UserSpokenLanguage
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public int LanguageId { get; set; }
        [ForeignKey(nameof(LanguageId))]
        public Language Language { get; set; }
    }
}
