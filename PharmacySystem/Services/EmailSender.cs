using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace PharmacySystem.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
                var port = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderName = _configuration["EmailSettings:SenderName"] ?? "PharmaCare";
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];

                if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(password))
                {
                    _logger.LogWarning("EmailSettings are missing or incomplete. Skipping email send.");
                    return;
                }

                using var client = new SmtpClient(smtpServer, port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true,
                    Timeout = 5000 // مهلة 5 ثوانٍ فقط لمنع تعليق الصفحة
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // تسجيل الخطأ لمنع سقوط السيرفر بـ Error 500
                _logger.LogError(ex, "Failed to send email to {Email}", email);
            }
        }
    }
}