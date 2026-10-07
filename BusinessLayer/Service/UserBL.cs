using BusinessLayer.Interface;
using Microsoft.AspNetCore.Identity;
using ModelLayer;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;

namespace BusinessLayer.Service
{
    public class UserBL : IUserBL
    {
        private readonly IUserRL _userRL;
        private readonly IPasswordHasher<UserEntity> _passwordHasher;
        private readonly JwtService _jwtService;
        private readonly IMessagingServiceClient _messagingServiceClient;

        public UserBL(
            IUserRL userRL,
            IPasswordHasher<UserEntity> passwordHasher,
            JwtService jwtService,
            IMessagingServiceClient messagingServiceClient)
        {
            _userRL = userRL;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _messagingServiceClient = messagingServiceClient;
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

        public async Task<bool> ForgotPasswordBL(ForgotPasswordModel forgotPasswordModel)
        {
            var user = _userRL.GetUserByEmailRL(
                forgotPasswordModel.Email
            );

            if (user == null)
            {
                return false;
            }

            var token = _jwtService.GenerateResetToken(user.Email, minutes: 15);

            user.ResetToken = token;
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);
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

            await _messagingServiceClient.SendEmailAsync(user.Email, subject, body);
            return true;
        }
        public bool ResetPasswordBL(ResetPasswordModel resetPasswordModel)
        {
            if (resetPasswordModel?.Token is null || string.IsNullOrWhiteSpace(resetPasswordModel.Email) || string.IsNullOrWhiteSpace(resetPasswordModel.NewPassword))
                return false;

            if (!_jwtService.TryValidateResetToken(resetPasswordModel.Token, out var tokenEmail))
                return false;

            if (!string.Equals(tokenEmail, resetPasswordModel.Email, StringComparison.OrdinalIgnoreCase))
                return false;

            var user = _userRL.GetUserByEmailRL(resetPasswordModel.Email);
            if (user == null || string.IsNullOrWhiteSpace(user.ResetToken) || user.ResetToken != resetPasswordModel.Token)
                return false;

            user.Password = _passwordHasher.HashPassword(user, resetPasswordModel.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;
            _userRL.UpdateUserRL(user);
            return true;
        }
    }
}