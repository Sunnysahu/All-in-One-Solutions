namespace Password_Hasing.Service
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        PasswordVerificationResult Verify(string password, string passwordHash);
    }
}