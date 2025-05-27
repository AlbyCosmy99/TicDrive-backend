namespace TicDrive.Dto.ServiceDto
{
    public class FullServiceDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? Bg_Image { get; set; }
        public int? FatherId { get; set; }
    }
}
