using TicDrive.Context;
using TicDrive.Models.Log;

namespace TicDrive.Utils.Auth
{
    public class LoginLogger
    {
        private readonly TicDriveDbContext _context;

        public LoginLogger(TicDriveDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string userId, bool success, string? failureReason, HttpContext httpContext)
        {
            var log = new LoginLog
            {
                UserId = userId,
                Success = success,
                FailureReason = failureReason,
                IPAddress = httpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = httpContext.Request.Headers["User-Agent"].ToString(),
                LoginTime = DateTime.UtcNow
            };

            _context.LoginLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
