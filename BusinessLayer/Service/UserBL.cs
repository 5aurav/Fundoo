using BusinessLayer.Interface;
using Middleware.Services.JWT;
using Middleware.Services.Email;
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
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IEmailSender _emailSender;

        public UserBL(
            IUserRL userRL,
            IPasswordHasher<UserEntity> passwordHasher,
            IJwtTokenService jwtTokenService,
            IEmailSender emailSender)
        {
            _userRL = userRL;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _emailSender = emailSender;
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

            if (verifyResult ==
                PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.Password = _passwordHasher.HashPassword(
                    user,
                    loginModel.password
                );

                _userRL.UpdateUserRL(user);
            }

            return _jwtTokenService.GenerateToken(user);
        }

        public async Task<bool> ForgotPasswordBL(
            ForgotPasswordModel forgotPasswordModel)
        {
            var user = _userRL.GetUserByEmailRL(
                forgotPasswordModel.Email
            );

            if (user == null)
            {
                return false;
            }

            var token = _jwtTokenService.GenerateResetToken(
                user.Email,
                minutes: 15
            );

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

            await _emailSender.SendEmailAsync(
                user.Email,
                subject,
                body
            );

            return true;
        }

        public bool ResetPasswordBL(
            ResetPasswordModel resetPasswordModel)
        {
            if (resetPasswordModel?.Token is null ||
                string.IsNullOrWhiteSpace(resetPasswordModel.Email) ||
                string.IsNullOrWhiteSpace(resetPasswordModel.NewPassword))
            {
                return false;
            }

            if (!_jwtTokenService.TryValidateResetToken(
                    resetPasswordModel.Token,
                    out var tokenEmail))
            {
                return false;
            }

            if (!string.Equals(
                    tokenEmail,
                    resetPasswordModel.Email,
                    StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var user = _userRL.GetUserByEmailRL(
                resetPasswordModel.Email
            );

            if (user == null ||
                string.IsNullOrWhiteSpace(user.ResetToken) ||
                user.ResetToken != resetPasswordModel.Token ||
                !user.ResetTokenExpiry.HasValue ||
                user.ResetTokenExpiry.Value <= DateTime.UtcNow)
            {
                return false;
            }

            user.Password = _passwordHasher.HashPassword(
                user,
                resetPasswordModel.NewPassword
            );

            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            _userRL.UpdateUserRL(user);

            return true;
        }
    }
}