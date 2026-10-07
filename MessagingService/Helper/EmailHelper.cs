using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace MessagingService.Helper
{
    public class EmailHelper
    {
        private readonly IConfiguration _configuration;

        public EmailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(
            string email,
            string subject,
            string body)
        {
            var emailSettings =
                _configuration.GetSection("EmailSettings");

            var senderEmail = emailSettings["Email"];
            var password = emailSettings["Password"];
            var smtpServer = emailSettings["SmtpServer"];

            var port = int.Parse(
                emailSettings["Port"] ?? "587"
            );

            var mail = new MailMessage();

            mail.From = new MailAddress(senderEmail);
            mail.To.Add(email);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            using var smtp = new SmtpClient(
                smtpServer,
                port);

            smtp.EnableSsl = true;

            smtp.Credentials = new NetworkCredential(
                senderEmail,
                password);

            await smtp.SendMailAsync(mail);
        }
    }
}