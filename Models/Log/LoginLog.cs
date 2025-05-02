using System.ComponentModel.DataAnnotations.Schema;

namespace TicDrive.Models.Log
{
    public class LoginLog
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        public DateTime LoginTime { get; set; } = DateTime.UtcNow;

        public string? IPAddress { get; set; }

        public string? UserAgent { get; set; }

        public bool Success { get; set; }

        public string? FailureReason { get; set; }
    }
}
