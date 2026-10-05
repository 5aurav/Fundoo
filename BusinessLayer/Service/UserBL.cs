using BusinessLayer.Interface;
using ModelLayer;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class UserBL : IUserBL
    {


        private readonly IUserRL _userRL;

        public UserBL(IUserRL userRL)
        {
            _userRL = userRL;
        }

        public RegistrationModel RegisterUserBL(RegistrationModel registrationModel)
        {
            return _userRL.RegisterUserRL(registrationModel);
        }
        public string? LoginUserBL(LoginModel loginModel)
        {
            return _userRL.LoginUserRL(loginModel);
        }
        
    }
}
