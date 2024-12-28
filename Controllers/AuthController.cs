using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using TicDrive.Enums;
using TicDrive.Models;
using TicDrive.Services;

namespace TicDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public AuthController(
            UserManager<User> userManager, 
            SignInManager<User> signInManager, 
            IAuthService authService,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
            _emailService = emailService;
        }

        public class RegisterBody
        {
            [Required]
            [MinLength(1, ErrorMessage = "Name must be at least 1 character long.")]
            public string Name { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
            public string Password { get; set; } = string.Empty;

            [Required]
            public string ConfirmPassword { get; set; } = string.Empty;
            [Required]
            [Range(1, int.MaxValue, ErrorMessage = "UserType must be set to a valid value.")]
            public UserType UserType { get; set; }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterBody payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (payload.Password != payload.ConfirmPassword)
                return BadRequest(new { Message = "Passwords do not match." });

            var user = new User
            {
                Name = payload.Name,
                Email = payload.Email,
                UserName = payload.Email,
                EmailConfirmed = false,
                UserType = payload.UserType
            };

            var result = await _userManager.CreateAsync(user, payload.Password);

            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(
                "ConfirmEmail",
                "Auth",
                new { userId = user.Id, token = emailConfirmationToken },
                Request.Scheme
            );

            if (string.IsNullOrEmpty(confirmationLink))
            {
                throw new Exception("Confirmation link is null or empty.");
            }

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            string body = @"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f4f4f4;
                            margin: 0;
                            padding: 0;
                        }}
                        .email-container {{
                            max-width: 600px;
                            margin: 30px auto;
                            background: #ffffff;
                            border-radius: 8px;
                            overflow: hidden;
                            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                        }}
                        .email-header {{
                            background: #00BF63;
                            color: #ffffff;
                            text-align: center;
                            padding: 20px;
                        }}
                        .email-header h1 {{
                            margin: 0;
                            font-size: 24px;
                        }}
                        .email-body {{
                            padding: 20px;
                            color: #333333;
                        }}
                        .email-body p {{
                            font-size: 16px;
                            line-height: 1.5;
                            margin: 0 0 15px;
                        }}
                        .email-footer {{
                            text-align: center;
                            font-size: 12px;
                            color: #777777;
                            padding: 10px;
                            background: #f4f4f4;
                        }}
                        .confirm-button {{
                            display: inline-block;
                            margin: 20px 0;
                            padding: 12px 25px;
                            font-size: 16px;
                            color: #ffffff;
                            background-color: #00BF63;
                            text-decoration: none;
                            border-radius: 5px;
                            font-weight: bold;
                        }}
                        .confirm-button:hover {{
                            background-color: #005bb5;
                        }}
                    </style>
                </head>
                <body>
                    <div class=""email-container"">
                        <div class=""email-header"">
                            <h1>TicDrive</h1>
                        </div>
                        <div class=""email-body"">
                            <p>Hello,</p>
                            <p>Thank you for signing up with TicDrive. To confirm your email address, please click the button below:</p>
                            <a class=""confirm-button"" href=""{0}"" target=""_blank"">Confirm Email</a>
                            <p>If the button above does not work, you can copy and paste the following link into your browser:</p>
                            <p><a href=""{0}"" target=""_blank"">{0}</a></p>
                            <p>Thank you,</p>
                            <p>The TicDrive Team</p>
                        </div>
                        <div class=""email-footer"">
                            &copy; 2024 TicDrive. All rights reserved.
                        </div>
                    </div>
                </body>
                </html>
            ";

            string formattedBody = string.Format(body, confirmationLink);

            await _emailService.SendEmailAsync(user.Email, "Welcome! Confirm your email.", formattedBody);

            var token = _authService.GenerateToken(user);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            });
        }
        public class LoginBody
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginBody payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid email or password." });
            }

            if (!user.EmailConfirmed)
            {
                return Unauthorized(new { Message = "Please confirm your email before logging in." });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, payload.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return Unauthorized(new { Message = "Invalid email or password." });
            }

            var token = _authService.GenerateToken(user);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            });
        }

        [HttpGet]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("UserId is required.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmailVerificationSuccess.html"), "text/html");
            }

            return BadRequest(result.Errors);
        }

        [HttpGet]
        [Authorize]
        [Route("check-email-confirmation")]
        public IActionResult CheckEmailConfirmation()
        {
            var email = User.Claims.Where(claim => claim.Type == "email").FirstOrDefault().Value;

            if(email == null)
            {
                return BadRequest("User email not found.");
            }
            
            if(_emailService.IsEmailConfirmed(email))
            {
                return Ok(new
                {
                    userId = User.Claims.Where(claim => claim.Type == "userId").FirstOrDefault().Value,
                    email,
                    name = User.Claims.Where(claim => claim.Type == "name").FirstOrDefault().Value

                });
            }

            return BadRequest("Email is not confirmed.");
        }
    }
}
