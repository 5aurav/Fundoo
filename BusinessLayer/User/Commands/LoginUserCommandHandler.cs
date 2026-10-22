
using Middleware.Services.JWT;
using Middleware.Services.Hashing;
using RepositoryLayer.Interface;

namespace BusinessLayer.User.Commands
{
    public class LoginUserCommandHandler
    {
        private readonly IUserRL _userRL;
        private readonly IPasswordHashing _passwordHashing;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginUserCommandHandler(
            IUserRL userRL,
            IPasswordHashing passwordHashing,
            IJwtTokenService jwtTokenService)
        {
            _userRL = userRL;
            _passwordHashing = passwordHashing;
            _jwtTokenService = jwtTokenService;
        }

        public string? Handle(LoginUserCommand command)
        {
            var model = command.LoginModel;

            if (string.IsNullOrWhiteSpace(model.email) ||
                string.IsNullOrWhiteSpace(model.password))
            {
                return null;
            }

            var user = _userRL.GetUserByEmailRL(model.email);

            if (user == null)
                return null;

            bool isPasswordValid =
                _passwordHashing.VerifyHashedPassword(
                    user.Password,
                    model.password);

            if (!isPasswordValid)
                return null;

            return _jwtTokenService.GenerateToken(user);
        }
    }
}
