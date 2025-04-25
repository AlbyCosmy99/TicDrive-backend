using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TicDrive.Context;
using TicDrive.Models;
using static TicDrive.Controllers.AuthController;

namespace TicDrive.Services
{
    public interface IAuthService
    {
        JwtSecurityToken GenerateToken(User user);
        Dictionary<string, string> GetUserClaims(ControllerBase controllerBase);
        string? GetUserEmail(Dictionary<string, string> userClaims);
        string? GetUserId(Dictionary<string, string> userClaims);
        string? GetUserName(Dictionary<string, string> userClaims);
        Task<User> GetUserData(string userId);
        Task UpdateUser(string userId, UpdatedUser updateUserQuery);
    }
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly TicDriveDbContext _context;
        public AuthService(IConfiguration configuration, TicDriveDbContext context) 
        {
            _configuration = configuration;
            _context = context;
        }
        public JwtSecurityToken GenerateToken(User user)
        {
            var authClaims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("name", user.Name),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                expires: DateTime.UtcNow.AddHours(24),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

        public Dictionary<string, string> GetUserClaims(ControllerBase controllerBase)
        {
            if (controllerBase == null)
            {
                throw new ArgumentNullException(nameof(controllerBase), "ControllerBase cannot be null.");
            }

            if (controllerBase.User == null || controllerBase.User.Claims == null)
            {
                throw new ArgumentNullException("User is not authenticated or claims are unavailable.");
            }

            var userClaims = controllerBase.User.Claims
                .Where(claim => claim.Type != null && claim.Value != null)
                .ToDictionary(claim => claim.Type, claim => claim.Value);

            return userClaims;
        }

        public string? GetUserEmail(Dictionary<string, string> userClaims) =>
            userClaims.TryGetValue("email", out var email) ? email : null;

        public string? GetUserId(Dictionary<string, string> userClaims) =>
            userClaims.TryGetValue("userId", out var userId) ? userId : null;

        public string? GetUserName(Dictionary<string, string> userClaims) => 
            userClaims.TryGetValue("name", out var name) ? name : null;

        public async Task<User> GetUserData(string userId) => await _context.Users.Where(user => user.UserType == Enums.UserType.Customer && user.Id == userId).FirstOrDefaultAsync();

        public async Task UpdateUser(string userId, UpdatedUser updateUserQuery)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID is required.", nameof(userId));

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new Exception("User not found.");

            if (!string.IsNullOrEmpty(updateUserQuery.Name))
                user.Name = updateUserQuery.Name;

            if (!string.IsNullOrEmpty(updateUserQuery.Address))
                user.Address = updateUserQuery.Address;

            if (updateUserQuery.Latitude.HasValue)
                user.Latitude = updateUserQuery.Latitude;

            if (updateUserQuery.Longitude.HasValue)
                user.Longitude = updateUserQuery.Longitude;

            if (!string.IsNullOrEmpty(updateUserQuery.ProfileImageUrl))
                user.ProfileImageUrl = updateUserQuery.ProfileImageUrl;

            if (!string.IsNullOrEmpty(updateUserQuery.PhoneNumber))
                user.PhoneNumber = updateUserQuery.PhoneNumber;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

    }
}
