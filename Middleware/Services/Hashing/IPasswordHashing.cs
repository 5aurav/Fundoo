
namespace Middleware.Services.Hashing
{
    public interface IPasswordHashing
    {
        string HashPassword(string password);

        bool VerifyHashedPassword(
            string storedHash,
            string providedPassword);
    }
}
