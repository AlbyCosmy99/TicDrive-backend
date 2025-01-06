
using Microsoft.AspNetCore.Identity;
using TicDrive.Enums;

namespace TicDrive.Models
{
    public class User : IdentityUser
    {
        public required string Name { get; set; }
        public override string? UserName { get; set; }
        public UserType UserType { get; set; }
        public string? Address { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
    }
}
