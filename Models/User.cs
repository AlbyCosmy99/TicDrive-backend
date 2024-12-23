using Microsoft.AspNetCore.Identity;

namespace TicDrive.Models
{
    public class User : IdentityUser
    {
        public required string Name { get; set; }
    }
}
