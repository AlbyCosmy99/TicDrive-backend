
using Microsoft.AspNetCore.Identity;
using System.Text;
using TicDrive.Enums;

namespace TicDrive.Models
{
    public class User : IdentityUser
    {
        public required string Name { get; set; }
        public string? Surname { get; set; }
        public override string? UserName { get; set; }
        public UserType UserType { get; set; }
        public string? Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
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
        public string ResetPasswordCode { get; set; } = string.Empty;
        public System.DateTime ResetPasswordExpiry { get; set; } = System.DateTime.UtcNow.AddMinutes(10);
        public string ResetPasswordToken { get; set; } = string.Empty;
        public System.DateTime? RegistrationDate { get; set; } = System.DateTime.UtcNow;
    }
}
