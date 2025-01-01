namespace TicDrive.Models
{
    public class Service
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; } = string.Empty;
        public string? Icon { get; set; }
    }
}
