using Middleware.Services.Email;
using Middleware.Services.JWT;
using ModelLayer;
using RepositoryLayer.Interface;

namespace BusinessLayer.User.Commands
{
    public class ForgotPasswordCommandHandler
    {
        private readonly IUserRL _userRL;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordCommandHandler(
            IUserRL userRL,
            IJwtTokenService jwtTokenService,
            IEmailSender emailSender)
        {
            _userRL = userRL;
            _jwtTokenService = jwtTokenService;
            _emailSender = emailSender;
        }

        public async Task<bool> Handle(
            ForgotPasswordCommand command)
        {
            var model = command.ForgotPasswordModel;

            var user = _userRL.GetUserByEmailRL(model.Email);

            if (user == null)
                return false;

            var token = _jwtTokenService.GenerateResetToken(
                user.Email,
                minutes: 15);

            user.ResetToken = token;
            user.ResetTokenExpiry =
                DateTime.UtcNow.AddMinutes(15);

            _userRL.UpdateUserRL(user);

            var subject = "Fundoo Password Reset";

            var body = $"""
                <h2>Password Reset Request</h2>
                <p>Hello {user.FirstName},</p>
                <p>We received a request to reset your Fundoo password.</p>
                <p>Your password reset token is:</p>
                <p><strong>{token}</strong></p>
                <p>This token will expire in 15 minutes.</p>
                <p>If you did not request a password reset, you can ignore this email.</p>
                """;

            await _emailSender.SendEmailAsync(
                user.Email,
                subject,
                body);

            return true;
        }
    }
}