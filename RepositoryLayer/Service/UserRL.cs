using Microsoft.AspNetCore.Identity;
using ModelLayer;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class UserRL : IUserRL
    {
        private readonly FundooContext fundooContext;
        private readonly IPasswordHasher<UserEntity> passwordHasher;
        private readonly JwtService jwtService;

        public UserRL(
            FundooContext fundooContext,
            IPasswordHasher<UserEntity> passwordHasher,
            JwtService jwtService)
        {
            this.fundooContext = fundooContext;
            this.passwordHasher = passwordHasher;
            this.jwtService = jwtService;
        }

        public RegistrationModel RegisterUserRL(
            RegistrationModel registrationModel)
        {
            UserEntity userEntity = new UserEntity();

            userEntity.FirstName = registrationModel.FirstName;
            userEntity.LastName = registrationModel.LastName;
            userEntity.Email = registrationModel.Email;
            userEntity.PhoneNumber = registrationModel.ContactNo;

            userEntity.Password = passwordHasher.HashPassword(
                userEntity,
                registrationModel.Password
            );

            fundooContext.Users.Add(userEntity);

            fundooContext.SaveChanges();

            registrationModel.Password = string.Empty;

            return registrationModel;
        }

        public string? LoginUserRL(LoginModel loginModel)
        {
            if (string.IsNullOrWhiteSpace(loginModel.email) ||
                string.IsNullOrWhiteSpace(loginModel.password))
            {
                return null;
            }

            var user = fundooContext.Users
                .FirstOrDefault(u => u.Email == loginModel.email);

            if (user == null)
            {
                return null;
            }

            var verifyResult = passwordHasher.VerifyHashedPassword(
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
                user.Password = passwordHasher.HashPassword(
                    user,
                    loginModel.password
                );

                fundooContext.SaveChanges();
            }

            string token = jwtService.GenerateToken(user);

            return token;
        }
    }
}