using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        UserEntity RegisterUserRL(UserEntity userEntity);
        UserEntity? GetUserByEmailRL(string email);
        UserEntity UpdateUserRL(UserEntity userEntity);
    }
}