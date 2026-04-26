
using Microsoft.EntityFrameworkCore;
using Password_Hasing.Repository;
using System.Security.Cryptography;
using System.Threading.Tasks;

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

            return $"pbkdf2-{Algorithm.Name?.ToLower()}-{Iterations}-{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
        }

        public PasswordVerificationResult Verify(string password, string passwordHash)
        {
            string[] parts = passwordHash.Split('-');
            byte[] hash;
            byte[] salt;

            if (parts.Length >= 5)
            {
                hash = Convert.FromHexString(parts[3]);
                salt = Convert.FromHexString(parts[4]);
            }

            else if (parts.Length == 2)
            {
                hash = Convert.FromHexString(parts[0]);
                salt = Convert.FromHexString(parts[1]);
            }
            else
            {
                return new PasswordVerificationResult
                {
                    Verified = false
                };
            }

            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

            bool isValid = CryptographicOperations.FixedTimeEquals(hash, inputHash);


            // Timing Attack -> A timing attack is a security risk where attackers try to guess a hash by measuring how long comparisons take. Normal comparison methods like SequenceEqual can stop early when a mismatch is found, revealing timing differences. To prevent this, constant-time comparison.

            // return hash.SequenceEqual(inputHash); 

            return new PasswordVerificationResult
            {
                Verified = isValid,
                NeedRehash = isValid && parts.Length == 2,
                NewHash = isValid && parts.Length == 2 ? Hash(password) : null
            };
        }
    }
}
