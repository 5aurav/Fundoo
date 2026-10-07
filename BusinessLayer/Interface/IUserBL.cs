using ModelLayer;

namespace BusinessLayer.Interface
{
    public interface IUserBL
    {
        RegistrationModel? RegisterUserBL(
            RegistrationModel registrationModel);

        string? LoginUserBL(
            LoginModel loginModel);

        Task<bool> ForgotPasswordBL(
            ForgotPasswordModel forgotPasswordModel);

        bool ResetPasswordBL(
            ResetPasswordModel resetPasswordModel);
    }
}