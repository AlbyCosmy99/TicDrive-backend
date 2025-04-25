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
                                ProfileImageUrl = "https://cdn1.iconfinder.com/data/icons/user-pictures/100/male3-512.png"
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

        public class ConfirmationEmail
        {
            public required string Email { get; set; }
        }

        [HttpPost]
        [Route("send-confirmation-email")]
        public async Task<IActionResult> SendConfirmationEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { Message = "Email is required." });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            if (user.EmailConfirmed)
            {
                return BadRequest(new { Message = "Email is already confirmed." });
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(
                "ConfirmEmail",
                "Auth",
                new { userId = user.Id, token },
                Request.Scheme
            );

            if (string.IsNullOrEmpty(confirmationLink))
            {
                return StatusCode(500, new { Message = "Unable to generate confirmation link." });
            }

            var bodyTemplate = _emailService.GetRegistrationMailConfirmation();
            var body = string.Format(bodyTemplate, confirmationLink);

            await _emailService.SendEmailAsync(user.Email, "Welcome! Confirm your email.", body);

            return Ok(new { Message = "Confirmation email has been resent." });
        }

        [HttpGet]
        [Route("check-email-is-confirmed")]
        public async Task<IActionResult> CheckEmailIsConfirmed([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { Message = "Email is required." });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            if (user.EmailConfirmed)
            {
                return NoContent();
            }

            return BadRequest(new { Message = "Email is not confirmed." });
        }


        public class LoginBody
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
            public string Password { get; set; } = string.Empty;
            public UserType UserType { get; set; }
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
                return Unauthorized("Invalid email or password.");
            }

            if(user.UserType != payload.UserType)
            {
                return Unauthorized("User is not authorized to login.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, payload.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return Unauthorized("Invalid email or password.");
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

            var resetCode = new Random().Next(100000, 999999).ToString();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            user.ResetPasswordCode = resetCode;
            user.ResetPasswordExpiry = DateTime.UtcNow.AddMinutes(10);
            user.ResetPasswordToken = token;
            await _userManager.UpdateAsync(user);

            var emailBody = $"Your password reset code is: <b>{resetCode}</b>. This code is valid for 10 minutes.";
            await _emailService.SendEmailAsync(user.Email, "Password Reset Code", emailBody);

            return Ok(new { Message = "If an account with that email exists, you will receive instructions to reset your password." });
        }


        public class ResetPasswordRequest
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
            public string NewPassword { get; set; } = string.Empty;

            [Required]
            [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;

        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest payload)
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

            var result = await _userManager.ResetPasswordAsync(user, user.ResetPasswordToken, payload.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Password reset successfully." });
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        public class SendCodePasswordForgotQuery
        {
            public string Email { get; set; } = string.Empty;
            public string Code { get; set; } = string.Empty;
        }

        [HttpPost]
        [Route("send-code-password-forgot")]
        public async Task<IActionResult> SendCodePasswordForgot([FromBody] SendCodePasswordForgotQuery query)
        {
            if(string.IsNullOrEmpty(query.Code))
            {
                return BadRequest("Code is required");
            }

            var user = await _userManager.FindByEmailAsync(query.Email);
            if (user == null)
            {
                return NotFound(new { Message = "Invalid request. User not found." });
            }

            if(user.ResetPasswordCode == query.Code)
            {
                return NoContent();
            }
            return Unauthorized("Invalid reset code");
        }

        public class UpdatedUser
        {
            public string? Name { get; set; }
            public string? Address { get; set; }
            public decimal? Latitude { get; set; }
            public decimal? Longitude { get; set; }
            public string? ProfileImageUrl { get; set; }
            public string? PhoneNumber { get; set; }
            public string? Email { get; set; }

        }

        [Route("update-user")]
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdatedUser newUser)
        {
            if(newUser == null)
            {
                return BadRequest("Params are required.");
            }
            var userClaims = _authService.GetUserClaims(this);
            var userId = _authService.GetUserId(userClaims);
            await _authService.UpdateUser(userId, newUser);

            return NoContent();
        }
    }
}
