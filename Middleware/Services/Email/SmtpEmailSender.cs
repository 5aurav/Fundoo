
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Net.Mail;

namespace Middleware.Services.Email
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var settings = _configuration.GetSection("EmailSettings");

            var senderEmail = settings["Email"];
            var password = settings["Password"];
            var smtpServer = settings["SmtpServer"];
            var port = int.Parse(settings["Port"] ?? "587");

            if (string.IsNullOrWhiteSpace(senderEmail) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(smtpServer))
            {
                throw new InvalidOperationException(
                    "Email settings are not configured correctly.");
            }

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(senderEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new BodyBuilder
            {
                HtmlBody = body
            }.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                await client.ConnectAsync(
                    smtpServer,
                    port,
                    SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(senderEmail, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            finally
            {
                if (client.IsConnected)
                    await client.DisconnectAsync(true);
            }
        }
    }
}
