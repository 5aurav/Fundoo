using ModelLayer;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        public RegistrationModel RegisterUserRL(RegistrationModel registrationModel);
        public string? LoginUserRL(LoginModel loginModel);
    }
}
