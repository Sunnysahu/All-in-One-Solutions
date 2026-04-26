using Password_Hasing.Models;

namespace Password_Hasing.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task AddAsync(User user);
        Task<bool> UpdateHash(string hash, int id);
    }
}
