using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicDrive.Models.Log
{
    public class LoginLog
    {
        [Key]
        public int Id { get; set; }

        public required string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        public DateTime LoginTime { get; set; } = DateTime.UtcNow;

        [MaxLength(45)] // Enough for IPv6
        public string? IPAddress { get; set; }

        [MaxLength(512)]
        public string? UserAgent { get; set; }

        public bool Success { get; set; }

        [MaxLength(512)]
        public string? FailureReason { get; set; }
    }
}
