using Middleware.Services.Hashing;
using Microsoft.AspNetCore.Identity;
using ModelLayer;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.User.Commands
{
    public class RegisterUserCommandHandler
    {
        private readonly IUserRL _userRL;
        private readonly IPasswordHashing _passwordHashing;
        public RegisterUserCommandHandler(
            IUserRL userRL,
            IPasswordHashing passwordHasher)
        {
            _userRL = userRL;
            _passwordHashing = passwordHasher;
        }

        public RegistrationModel? Handle(
            RegisterUserCommand command)
        {
            var registrationModel = command.RegistrationModel;

            var existingUser = _userRL.GetUserByEmailRL(
                registrationModel.Email);

            if (existingUser != null)
                return null;

            var userEntity = new UserEntity
            {
                FirstName = registrationModel.FirstName,
                LastName = registrationModel.LastName,
                Email = registrationModel.Email,
                PhoneNumber = registrationModel.ContactNo
            };

            userEntity.Password = _passwordHashing.HashPassword(
                registrationModel.Password!);

            _userRL.RegisterUserRL(userEntity);

            registrationModel.Password = string.Empty;

            return registrationModel;
        }
    }
}