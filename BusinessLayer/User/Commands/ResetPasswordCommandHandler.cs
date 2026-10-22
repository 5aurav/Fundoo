
using Middleware.Services.JWT;
using Middleware.Services.Hashing;
using RepositoryLayer.Interface;

namespace BusinessLayer.User.Commands
{
    public class ResetPasswordCommandHandler
    {
        private readonly IUserRL _userRL;
        private readonly IPasswordHashing _passwordHashing;
        private readonly IJwtTokenService _jwtTokenService;

        public ResetPasswordCommandHandler(
            IUserRL userRL,
            IPasswordHashing passwordHashing,
            IJwtTokenService jwtTokenService)
        {
            _userRL = userRL;
            _passwordHashing = passwordHashing;
            _jwtTokenService = jwtTokenService;
        }

        public bool Handle(ResetPasswordCommand command)
        {
            var model = command.ResetPasswordModel;

            if (model?.Token is null ||
                string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.NewPassword))
            {
                return false;
            }

            if (!_jwtTokenService.TryValidateResetToken(
                    model.Token,
                    out var tokenEmail))
            {
                return false;
            }

            if (!string.Equals(
                    tokenEmail,
                    model.Email,
                    StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var user = _userRL.GetUserByEmailRL(model.Email);

            if (user == null ||
                string.IsNullOrWhiteSpace(user.ResetToken) ||
                user.ResetToken != model.Token ||
                !user.ResetTokenExpiry.HasValue ||
                user.ResetTokenExpiry.Value <= DateTime.UtcNow)
            {
                return false;
            }

            user.Password = _passwordHashing.HashPassword(
                model.NewPassword);

            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            _userRL.UpdateUserRL(user);

            return true;
        }
    }
}
