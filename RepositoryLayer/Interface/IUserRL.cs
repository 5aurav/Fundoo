using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        UserEntity RegisterUserRL(UserEntity userEntity);

        UserEntity? GetUserByEmailRL(string email);

        UserEntity? GetUserByResetTokenRL(string token);

        UserEntity UpdateUserRL(UserEntity userEntity);
    }
}