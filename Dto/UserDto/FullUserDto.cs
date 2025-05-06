using TicDrive.Dto.UserImageDto;
using TicDrive.Enums;
using TicDrive.Interfaces.Dto;

namespace TicDrive.Dto.UserDto
{
    public class FullUserDto : IUserDto
    {
        public string Id { get; set; }
        public UserType UserType { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public bool? EmailConfirmed { get; set; }
        public List<FullUserImageDto> Images { get; set; } = [];
    }
}
