using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class UserRL : IUserRL
    {
        private readonly FundooContext fundooContext;

        public UserRL(FundooContext fundooContext)
        {
            this.fundooContext = fundooContext;
        }

        public UserEntity RegisterUserRL(UserEntity userEntity)
        {
            fundooContext.Users.Add(userEntity);
            fundooContext.SaveChanges();

            return userEntity;
        }

        public UserEntity? GetUserByEmailRL(string email)
        {
            return fundooContext.Users
                .FirstOrDefault(user => user.Email == email);
        }

        public UserEntity? GetUserByResetTokenRL(string token)
        {
            return fundooContext.Users
                .FirstOrDefault(user => user.ResetToken == token);
        }

        public UserEntity UpdateUserRL(UserEntity userEntity)
        {
            fundooContext.Users.Update(userEntity);
            fundooContext.SaveChanges();

            return userEntity;
        }
    }
}