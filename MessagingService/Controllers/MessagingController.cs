using MessagingService.Interface;
using MessagingService.Models;
using Microsoft.AspNetCore.Mvc;

namespace MessagingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagingController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagingController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail(
            [FromBody] EmailModel emailModel)
        {
            await _messageService.SendEmailAsync(
                emailModel.To,
                emailModel.Subject,
                emailModel.Body
            );

            return Ok("Email sent successfully");
        }
    }
}
