using MailKit.Net.Smtp;
using MimeKit;
using TicDrive.Context;


namespace TicDrive.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        bool IsEmailConfirmed(string? email);
        string GetRegistrationMailConfirmation();
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

        public bool IsEmailConfirmed(string? email)
        {
            var user = _dbContext.Users.Where(user => user.Email == email).FirstOrDefault();

            if (user == null) return false;

            if (user.EmailConfirmed) return true;

            return false;
        }

        public string GetRegistrationMailConfirmation()
        {
            return @"
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
        }
    }

}
