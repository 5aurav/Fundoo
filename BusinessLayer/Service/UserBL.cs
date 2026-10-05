using BusinessLayer.Interface;
using Microsoft.AspNetCore.Identity;
using ModelLayer;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class UserBL : IUserBL
    {
        private readonly IUserRL _userRL;
        private readonly IPasswordHasher<UserEntity> _passwordHasher;
        private readonly JwtService _jwtService;

        public UserBL(
            IUserRL userRL,
            IPasswordHasher<UserEntity> passwordHasher,
            JwtService jwtService)
        {
            _userRL = userRL;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public RegistrationModel? RegisterUserBL(
    RegistrationModel registrationModel)
        {
            var existingUser = _userRL.GetUserByEmailRL(
                registrationModel.Email
            );

            if (existingUser != null)
            {
                return null;
            }

            UserEntity userEntity = new UserEntity
            {
                FirstName = registrationModel.FirstName,
                LastName = registrationModel.LastName,
                Email = registrationModel.Email,
                PhoneNumber = registrationModel.ContactNo
            };

            userEntity.Password = _passwordHasher.HashPassword(
                userEntity,
                registrationModel.Password!
            );

            _userRL.RegisterUserRL(userEntity);

            registrationModel.Password = string.Empty;

            return registrationModel;
        }

        public string? LoginUserBL(LoginModel loginModel)
        {
            if (string.IsNullOrWhiteSpace(loginModel.email) ||
                string.IsNullOrWhiteSpace(loginModel.password))
            {
                return null;
            }

            var user = _userRL.GetUserByEmailRL(loginModel.email);

            if (user == null)
            {
                return null;
            }

            var verifyResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.Password,
                loginModel.password
            );

            if (verifyResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            if (verifyResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.Password = _passwordHasher.HashPassword(
                    user,
                    loginModel.password
                );

                _userRL.UpdateUserRL(user);
            }

            return _jwtService.GenerateToken(user);
        }
    }
}