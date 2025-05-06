using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TicDrive.Context;
using TicDrive.Dto.UserDto;
using TicDrive.Dto.UserDto.WorkshopDto;
using TicDrive.Dto.UserImageDto;
using TicDrive.Enums;
using TicDrive.Interfaces.Dto;
using TicDrive.Models;
using TicDrive.Models.Workshops;
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
        UserType? GetUserType(Dictionary<string, string> userClaims);
        Task<IUserDto?> GetUserData(UserType userType, string userId);
        Task UpdateUser(string userId, UpdatedUser updateUserQuery);
        Task RegisterWorkshop(string userId, string workshopName, List<int> specializations, List<int> services, Dictionary<int, ScheduleEntry> schedule, string description, int laborWarrantyMonths, int maxDailyVehicles, bool offersHomeServices, string signatureName, string signatureSurname);
    }
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly TicDriveDbContext _context;
        private readonly IImagesService _imagesService;
        private readonly IMapper _mapper;
        public AuthService(IConfiguration configuration, TicDriveDbContext context, IImagesService imagesService, IMapper mapper)
        {
            _configuration = configuration;
            _context = context;
            _imagesService = imagesService;
            _mapper = mapper;
        }
        public JwtSecurityToken GenerateToken(User user)
        {
            var authClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("name", user.Name),
                new Claim("userType", user.UserType.ToString()),
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

        public UserType? GetUserType(Dictionary<string, string> userClaims)
        {
            if (userClaims.TryGetValue("userType", out var userTypeStr) &&
                Enum.TryParse<UserType>(userTypeStr, true, out var userType))
            {
                return userType;
            }

            return null;
        }

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

            if (!string.IsNullOrEmpty(updateUserQuery.PhoneNumber))
                user.PhoneNumber = updateUserQuery.PhoneNumber;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task RegisterWorkshop(
            string userId,
            string workshopName,
            List<int> specializations,
            List<int> services,
            Dictionary<int, ScheduleEntry> schedule,
            string description, 
            int laborWarrantyMonths,
            int maxDailyVehicles, 
            bool offersHomeServices,
            string signatureName, 
            string signatureSurname)
        {
            var workshopDetails = new WorkshopDetails
            {
                WorkshopId = userId,
                WorkshopName = workshopName,
                Description = description,
                LaborWarrantyMonths = laborWarrantyMonths,
                MaxDailyVehicles = maxDailyVehicles,
                OffersHomeServices = offersHomeServices,
                SignatureName = signatureName,
                SignatureSurname = signatureSurname,    
                SignatureDate = System.DateTime.UtcNow
            };

            _context.WorkshopsDetails.Add(workshopDetails);

            foreach (var specialization in specializations)
            {
                _context.WorkshopsSpecializations.Add(new WorkshopSpecialization
                {
                    WorkshopId = userId,
                    SpecializationId = specialization
                });
            }

            foreach (var service in services)
            {
                _context.OfferedServices.Add(new OfferedServices
                {
                    WorkshopId = userId,
                    ServiceId = service
                });
            }

            foreach (var kvp in schedule)
            {
                int dayId = kvp.Key;
                var entry = kvp.Value;

                _context.WorkshopsSchedules.Add(new WorkshopSchedule
                {
                    WorkshopId = userId,
                    DayId = dayId,
                    MorningStartTime = entry.MorningStartTime,
                    MorningEndTime = entry.MorningEndTime,
                    AfternoonStartTime = entry.AfternoonStartTime,
                    AfternoonEndTime = entry.AfternoonEndTime
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IUserDto?> GetUserData(UserType userType, string userId)
        {
            var images = await _imagesService.GetUserImagesAsync(userId, 5);
            var mappedImages = _mapper.Map<List<FullUserImageDto>>(images);
            if (userType == UserType.Workshop)
            {
                return await _context.Users
                    .Where(user => user.Id == userId)
                    .Join(_context.WorkshopsDetails,
                        user => user.Id,
                        workshop => workshop.WorkshopId,
                        (user, workshop) => new FullWorkshopDto
                        {
                            Id = user.Id,
                            Name = user.Name,
                            Surname = user.Surname,
                            UserType = userType,
                            Email = user.Email,
                            EmailConfirmed = user.EmailConfirmed,
                            PhoneNumber = user.PhoneNumber,
                            WorkshopName = workshop.WorkshopName,
                            Address = user.Address,
                            Latitude = user.Latitude,
                            Longitude = user.Longitude,
                            Images = mappedImages
                        })
                    .FirstOrDefaultAsync();
            }

            return await _context.Users
                .Where(user => user.Id == userId)
                .Select(user => new FullUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    UserType = userType,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumber = user.PhoneNumber,
                    Images = mappedImages
                })
                .FirstOrDefaultAsync();
        }
    }
}
