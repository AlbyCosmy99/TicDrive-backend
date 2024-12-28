using MailKit.Net.Smtp;
using MimeKit;
using TicDrive.Context;


namespace TicDrive.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        bool IsEmailConfirmed(string email);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly TicDriveDbContext _dbContext;

        public EmailService(IConfiguration configuration, TicDriveDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("TicDrive", _configuration["Email:From"]));
            email.To.Add(new MailboxAddress("", to));
            email.Subject = subject;
            email.Body = new TextPart("html")
            {
                Text = body
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_configuration["Email:SmtpServer"], int.Parse(_configuration["Email:SmtpPort"]), true);
            await smtp.AuthenticateAsync(_configuration["Email:Username"], _configuration["Email:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public bool IsEmailConfirmed(string email)
        {
            var user = _dbContext.Users.Where(user => user.Email == email).FirstOrDefault();

            if (user == null) return false;

            if (user.EmailConfirmed) return true;

            return false;
        }
    }

}
