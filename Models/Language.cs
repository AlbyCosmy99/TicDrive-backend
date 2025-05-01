using TicDrive.Enums;

namespace TicDrive.Models
{
    public class Language
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
    }
}
