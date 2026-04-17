using JWT.Models;

namespace JWT.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetUserAsync(string username, string password);
    }
}
