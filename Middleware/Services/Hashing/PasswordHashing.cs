
using System.Security.Cryptography;

namespace Middleware.Services.Hashing
{
    public class PasswordHashing : IPasswordHashing
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 600000;

        public string HashPassword(string password)
        {
            ArgumentNullException.ThrowIfNull(password);

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                HashSize
            );

            return $"fundoo-pbkdf2-sha256$v=1$i={Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        public bool VerifyHashedPassword(
            string storedHash,
            string providedPassword)
        {
            if (string.IsNullOrWhiteSpace(storedHash) ||
                providedPassword == null)
            {
                return false;
            }

            string[] parts = storedHash.Split('$');

            if (parts.Length != 5 ||
                parts[0] != "fundoo-pbkdf2-sha256" ||
                parts[1] != "v=1" ||
                !parts[2].StartsWith("i="))
            {
                return false;
            }

            if (!int.TryParse(parts[2][2..], out int iterations) ||
                iterations < 1 ||
                iterations > 1000000)
            {
                return false;
            }

            try
            {
                byte[] salt = Convert.FromBase64String(parts[3]);
                byte[] storedHashBytes = Convert.FromBase64String(parts[4]);

                if (salt.Length != SaltSize ||
                    storedHashBytes.Length != HashSize)
                {
                    return false;
                }

                byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(
                    providedPassword,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256,
                    HashSize
                );

                return CryptographicOperations.FixedTimeEquals(
                    computedHash,
                    storedHashBytes
                );
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
