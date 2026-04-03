using System.Security.Cryptography;

namespace Pg.DataverseSync.Api.Domain
{
    public class User
    {
        private const int _passwordHashIterations = 100_000;
        private const short _outputLength = 32;
        private readonly HashAlgorithmName _passwordHashAlgorithm = HashAlgorithmName.SHA256;

        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
        public DateTime CreatedOn { get; set; }

        public void  GeneratePasswordHash(string password)
        {
            PasswordSalt = RandomNumberGenerator.GetBytes(16);
            PasswordHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                PasswordSalt,
                _passwordHashIterations,
                _passwordHashAlgorithm,
                _outputLength);
        }

        public bool VerifyPassword(string password)
        {
            var computedHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                PasswordSalt,
                _passwordHashIterations,
                _passwordHashAlgorithm,
                _outputLength);

            return CryptographicOperations.FixedTimeEquals(computedHash, PasswordHash);
        }
    }
}
