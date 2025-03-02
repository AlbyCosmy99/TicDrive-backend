using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using TicDrive.Attributes;
using TicDrive.Context;
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
        private readonly TicDriveDbContext _context;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IAuthService authService,
            IEmailService emailService,
            TicDriveDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
            _emailService = emailService;
            _context = context;
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

            [RequiredIfUserType(2, ErrorMessage = "Address is required when UserType is 2 (workshop).")]
            public string? Address { get; set; }

            [RequiredIfUserType(2, ErrorMessage = "Latitude is required when UserType is 2 (workshop).")]
            public decimal? Latitudine { get; set; }

            [RequiredIfUserType(2, ErrorMessage = "Longitude is required when UserType is 2 (workshop).")]
            public decimal? Longitude { get; set; }
            public bool? IsVerified { get; set; }
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

            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync<object, IActionResult>(
                state: null,
                operation: async (dbContext, state, cancellationToken) =>
                {
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            var user = new User
                            {
                                Name = payload.Name,
                                Email = payload.Email,
                                UserName = payload.Email,
                                EmailConfirmed = false,
                                UserType = payload.UserType,
                                IsVerified = payload.IsVerified,
                            };

                            var result = await _userManager.CreateAsync(user, payload.Password);

                            if (!result.Succeeded)
                            {
                                return BadRequest(result.Errors);
                            }

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

                            var body = _emailService.GetRegistrationMailConfirmation();
                            string formattedBody = string.Format(body, confirmationLink);
                            await _emailService.SendEmailAsync(user.Email, "Welcome! Confirm your email.", formattedBody);

                            var token = _authService.GenerateToken(user);
                            await _context.Database.CommitTransactionAsync();
                            return Ok(new
                            {
                                Token = new JwtSecurityTokenHandler().WriteToken(token),
                                Expiration = token.ValidTo
                            });
                        }
                        catch (Exception ex)
                        {
                            await _context.Database.RollbackTransactionAsync();
                            return StatusCode(500, new { Message = "Registration failed", Details = ex.Message });
                        }
                    }
                },
                verifySucceeded: null
            );
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

            var result = await _signInManager.CheckPasswordSignInAsync(user, payload.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return Unauthorized(new { Message = "Invalid email or password." });
            }

            var token = _authService.GenerateToken(user);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                user.EmailConfirmed
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
            try
            {
                var userClaims = _authService.GetUserClaims(this);
                var email = _authService.GetUserEmail(userClaims);
                var userId = _authService.GetUserId(userClaims);
                var name = _authService.GetUserName(userClaims);

                if (_emailService.IsEmailConfirmed(email))
                {
                    return Ok(new
                    {
                        emailConfirmed = true,
                        userId,
                        email,
                        name
                    });
                }

                return BadRequest("Email is not confirmed.");
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("get-payload")]
        public async Task<IActionResult> GetPayload()
        {
            try
            {
                var userClaims = _authService.GetUserClaims(this);
                var email = _authService.GetUserEmail(userClaims);
                var userId = _authService.GetUserId(userClaims);
                var name = _authService.GetUserName(userClaims);

                var userData = await _authService.GetUserData(userId);

                return Ok(new
                {
                    emailConfirmed = email != null && _emailService.IsEmailConfirmed(email),
                    userId,
                    email,
                    name,
                    imageUrl = userData.ProfileImageUrl,
                    phoneNumber = userData.PhoneNumber,
                    address = userData.Address
                });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ----- Forgot Password API Endpoints -----

        public class ForgotPasswordRequest
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null || !user.EmailConfirmed)
            {
                return Ok(new { Message = "If an account with that email exists, you will receive instructions to reset your password." });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = Url.Action("ResetPassword", "Auth", new { email = user.Email, token }, Request.Scheme);
            if (string.IsNullOrEmpty(resetLink))
            {
                return StatusCode(500, new { Message = "Could not generate password reset link." });
            }

            var emailBody = $"Please reset your password by clicking the following link: {resetLink}";
            await _emailService.SendEmailAsync(user.Email, "Password Reset Request", emailBody);

            return Ok(new { Message = "If an account with that email exists, you will receive instructions to reset your password." });
        }

        public class ResetPasswordRequest
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Token { get; set; } = string.Empty;

            [Required]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
            public string NewPassword { get; set; } = string.Empty;

            [Required]
            [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;

        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordRequest payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                return BadRequest(new { Message = "Invalid request." });
            }

            var result = await _userManager.ResetPasswordAsync(user, payload.Token, payload.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Password reset successfully." });
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpGet]
        [Route("reset-password")]
        public IActionResult ResetPasswordForm([FromQuery] string email, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Email and token are required.");
            }

            var html = $@"
            <html>
            <head>
                <meta charset='UTF-8'>
                <title>Reset Password</title>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f2f2f2;
                        margin: 0;
                        height: 100vh;
                        display: flex;
                        align-items: center;
                        justify-content: center;
                    }}
                    .modal {{
                        background-color: #fff;
                        padding: 30px;
                        border-radius: 8px;
                        box-shadow: 0 4px 8px rgba(0,0,0,0.2);
                        max-width: 400px;
                        width: 90%;
                    }}
                    .modal h2 {{
                        text-align: center;
                        color: green;
                        margin-bottom: 20px;
                    }}
                    .modal form div {{
                        margin-bottom: 15px;
                    }}
                    .modal label {{
                        display: block;
                        margin-bottom: 5px;
                        font-weight: bold;
                    }}
                    .modal input[type='password'] {{
                        width: 100%;
                        padding: 10px;
                        border: 1px solid #ccc;
                        border-radius: 4px;
                        box-sizing: border-box;
                    }}
                    .modal button {{
                        width: 100%;
                        padding: 12px;
                        background-color: green;
                        color: #fff;
                        border: none;
                        border-radius: 4px;
                        cursor: pointer;
                        font-size: 16px;
                    }}
                    .modal button:hover {{
                        background-color: #006400;
                    }}
                </style>
            </head>
            <body>
                <div class='modal'>
                    <h2>Reset Your Password</h2>
                    <form method='post' action='/api/auth/reset-password'>
                        <input type='hidden' name='Email' value='{email}' />
                        <input type='hidden' name='Token' value='{token}' />
                        <div>
                            <label for='NewPassword'>New Password:</label>
                            <input type='password' id='NewPassword' name='NewPassword' required />
                        </div>
                        <div>
                            <label for='ConfirmPassword'>Confirm Password:</label>
                            <input type='password' id='ConfirmPassword' name='ConfirmPassword' required />
                        </div>
                        <button type='submit'>Reset Password</button>
                    </form>
                </div>
            </body>
            </html>";

            return Content(html, "text/html");
        }
    }
}
