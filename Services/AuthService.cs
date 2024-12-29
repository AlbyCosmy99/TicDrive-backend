using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TicDrive.Models;

namespace TicDrive.Services
{
    public interface IAuthService
    {
        JwtSecurityToken GenerateToken(User user);
        Dictionary<string, string> GetUserInfo(ControllerBase controllerBase);
    }
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        public AuthService(IConfiguration configuration) 
        {
            _configuration = configuration;
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

        public Dictionary<string, string> GetUserInfo(ControllerBase controllerBase)
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

    }
}
