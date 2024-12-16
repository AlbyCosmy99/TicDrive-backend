namespace TicDrive.Models
{
    public class Service
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string Description { get; set; } = "";
    }
}
