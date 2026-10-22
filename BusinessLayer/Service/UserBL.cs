using BusinessLayer.Interface;
using BusinessLayer.User.Commands;
using ModelLayer;

namespace BusinessLayer.Service
{
    public class UserBL : IUserBL
    {
        private readonly RegisterUserCommandHandler _registerUserHandler;
        private readonly LoginUserCommandHandler _loginUserHandler;
        private readonly ForgotPasswordCommandHandler _forgotPasswordHandler;
        private readonly ResetPasswordCommandHandler _resetPasswordHandler;

        public UserBL(
            RegisterUserCommandHandler registerUserHandler,
            LoginUserCommandHandler loginUserHandler,
            ForgotPasswordCommandHandler forgotPasswordHandler,
            ResetPasswordCommandHandler resetPasswordHandler)
        {
            _registerUserHandler = registerUserHandler;
            _loginUserHandler = loginUserHandler;
            _forgotPasswordHandler = forgotPasswordHandler;
            _resetPasswordHandler = resetPasswordHandler;
        }

        public RegistrationModel? RegisterUserBL(
            RegistrationModel registrationModel)
        {
            var command = new RegisterUserCommand(
                registrationModel);

            return _registerUserHandler.Handle(command);
        }

        public string? LoginUserBL(LoginModel loginModel)
        {
            var command = new LoginUserCommand(loginModel);

            return _loginUserHandler.Handle(command);
        }

        public Task<bool> ForgotPasswordBL(
            ForgotPasswordModel forgotPasswordModel)
        {
            var command = new ForgotPasswordCommand(
                forgotPasswordModel);

            return _forgotPasswordHandler.Handle(command);
        }

        public bool ResetPasswordBL(
            ResetPasswordModel resetPasswordModel)
        {
            var command = new ResetPasswordCommand(
                resetPasswordModel);

            return _resetPasswordHandler.Handle(command);
        }
    }
}