using System.ComponentModel.DataAnnotations;
using TicDrive.Enums;

namespace TicDrive.Models.Legal
{
    public class LegalDeclaration
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public System.DateTime Issued { get; set; }

        public string? Version { get; set; }

        public string? Content { get; set; }
        public string? Url { get; set; }

        public bool IsActive { get; set; } = true;
        public LegalDocumentContext? Context { get; set; } = LegalDocumentContext.AllEcosystem;
        public LegalDocumentType Type { get; set; }
    }
}
