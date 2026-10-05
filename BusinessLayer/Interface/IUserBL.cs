using ModelLayer;

namespace BusinessLayer.Interface
{
    public interface IUserBL
    {
        public RegistrationModel RegisterUserBL(RegistrationModel registrationModel);
        public string? LoginUserBL(LoginModel loginModel);
    }
}
