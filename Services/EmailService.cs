using Azure.Core;
using Azure.Identity;
using MailKit.Net.Smtp;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;
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
        private readonly string _senderEmail;
        private readonly string _senderPassword;
        private readonly TicDriveDbContext _dbContext;

        public EmailService(IConfiguration config, TicDriveDbContext dbContext)
        {
            _dbContext = dbContext;
            _senderEmail = config["EmailSettings:SenderEmail"]!;
            _senderPassword = config["EmailSettings:SenderPassword"]!;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("TicDrive", _senderEmail));
            message.To.Add(new MailboxAddress(to, to));
            message.Subject = subject;

            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_senderEmail, _senderPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public bool IsEmailConfirmed(string? email)
        {
            return _dbContext.Users.FirstOrDefault(u => u.Email == email)?.EmailConfirmed ?? false;
        }

        public string GetRegistrationMailConfirmation()
        {
            return @"<!DOCTYPE html>
                <html lang=""it"">
                <head><meta charset=""UTF-8""><meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Conferma la tua registrazione</title>
                <style>
                    body {{ font-family: 'Segoe UI', Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; color: #333; }}
                    .email-container {{ max-width: 600px; margin: 40px auto; background-color: #fff; border-radius: 8px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }}
                    .email-header {{ background-color: #00BF63; padding: 24px; text-align: center; }}
                    .email-header h1 {{ color: #fff; margin: 0; font-size: 26px; }}
                    .email-body {{ padding: 24px; }}
                    .email-body p {{ font-size: 16px; line-height: 1.6; margin: 0 0 16px; }}
                    .confirm-button {{ display: inline-block; padding: 14px 28px; background-color: #00BF63; color: #fff !important;
                        text-decoration: none; border-radius: 6px; font-weight: 600; font-size: 16px; transition: background-color 0.3s ease; }}
                    .confirm-button:hover {{ background-color: #00994e; text-decoration: none; }}
                    .email-footer {{ background-color: #f4f4f4; padding: 16px; text-align: center; font-size: 13px; color: #888; }}
                    a {{ color: #00BF63; word-break: break-word; }} a:hover {{ text-decoration: underline; color: #00994e; }}
                </style>
                </head>
                <body>
                <div class=""email-container"">
                    <div class=""email-header""><h1>TicDrive</h1></div>
                    <div class=""email-body"">
                        <p>Ciao,</p>
                        <p>Grazie per esserti registrato su <strong>TicDrive</strong>. Per completare la registrazione, conferma il tuo indirizzo email cliccando sul pulsante qui sotto:</p>
                        <p style=""text-align: center;"">
                            <a class=""confirm-button"" href=""{0}"" target=""_blank"">Conferma Email</a>
                        </p>
                        <p>Se il pulsante non funziona, copia e incolla questo link nel tuo browser:</p>
                        <p><a href=""{0}"" target=""_blank"">{0}</a></p>
                        <p>Grazie per aver scelto TicDrive!</p>
                        <p>Il team di TicDrive</p>
                    </div>
                    <div class=""email-footer"">&copy; 2024 TicDrive. Tutti i diritti riservati.</div>
                </div>
                </body>
                </html>";
        }

    }
}
