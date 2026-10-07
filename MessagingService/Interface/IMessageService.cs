using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingService.Interface
{
    public interface IMessageService
    {
        Task SendEmailAsync(string email, string subject, string body);
    }
}