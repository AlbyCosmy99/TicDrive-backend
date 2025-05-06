using TicDrive.Dto.UserImageDto;
using TicDrive.Enums;

namespace TicDrive.Interfaces.Dto
{
    public interface IUserDto
    {
        string Id { get; set; }
        public UserType UserType { get; set; }
        string? Name { get; set; }
        string? Surname { get; set; }
        string? Address { get; set; }
        decimal? Latitude { get; set; }
        decimal? Longitude { get; set; }
        string? PhoneNumber { get; set; }
        string? Email { get; set; }
        bool? EmailConfirmed { get; set; }
        List<FullUserImageDto> Images { get; set; }
    }
}
