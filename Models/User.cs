
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
        public string? ProfileImageUrl { get; set; }
        private bool? _isVerified;
        public bool? IsVerified
        {
            get => _isVerified;
            set
            {
                if (UserType != UserType.Workshop)
                {
                    _isVerified = null;
                }
                else
                {
                    _isVerified = value ?? false;
                }
            }
        }
    }
}
