using System.ComponentModel.DataAnnotations.Schema;
using TicDrive.Models.Legal;

namespace TicDrive.Models
{
    public class UserConsent
    {
        public int Id { get; set; }
        public string UserId {get; set;}
        [ForeignKey(nameof(UserId))]
        public User User { get; set;}
        public int LegalDeclarationId { get; set;}
        [ForeignKey(nameof(LegalDeclarationId))]
        public LegalDeclaration LegalDeclaration { get; set;}
        public System.DateTime When { get; set;}
    }
}
