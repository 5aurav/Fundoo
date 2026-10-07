using BusinessLayer.Interface;
using System.Net.Http.Json;

namespace BusinessLayer.Service
{
    public class MessagingServiceClient : IMessagingServiceClient
    {
        private readonly HttpClient _httpClient;

        public MessagingServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendEmailAsync(
            string to,
            string subject,
            string body)
        {
            var email = new
            {
                To = to,
                Subject = subject,
                Body = body
            };

            await _httpClient.PostAsJsonAsync(
                "api/Messaging/send-email",
                email
            );
        }
    }
}