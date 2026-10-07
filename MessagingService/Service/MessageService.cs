using MessagingService.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagingService.Helper;

namespace MessagingService.Service
{
    public class MessageService : IMessageService
    {
        private readonly EmailHelper _emailHelper;

        public MessageService(EmailHelper emailHelper)
        {
            _emailHelper = emailHelper;
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            await _emailHelper.SendEmailAsync(
                email,
                subject,
                body
            );
        }
    }
}