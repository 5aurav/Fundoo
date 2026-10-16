using RepositoryLayer.Entity;

namespace Middleware.Services.JWT
{
    public interface IJwtTokenService
    {
        string GenerateToken(UserEntity user);

        string GenerateResetToken(string email, int minutes = 15);

        bool TryValidateResetToken(string token, out string? email);
    }
}