
using System.Security.Cryptography;

namespace Password_Hasing.Service
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;

        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;


        public string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

            return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
        }
        // 7:24

        public bool Verify(string password, string passwordHash)
        {
            string[] parts = passwordHash.Split('-');
            byte[] hash = Convert.FromHexString(parts[0]);
            byte[] salt = Convert.FromHexString(parts[1]);

            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

            // Timing Attack -> A timing attack is a security risk where attackers try to guess a hash by measuring how long comparisons take. Normal comparison methods like SequenceEqual can stop early when a mismatch is found, revealing timing differences. To prevent this, constant-time comparison.

            // return hash.SequenceEqual(inputHash); 

            return CryptographicOperations.FixedTimeEquals(hash, inputHash);
        }
    }
}
