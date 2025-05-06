namespace TicDrive.Dto.UserImageDto
{
    public class FullUserImageDto
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string Url { get; set; }
        public bool? IsMainImage { get; set; } = false;
    }
}
