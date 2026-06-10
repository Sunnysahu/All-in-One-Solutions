using GlobalAnnotationUtils.Models;

namespace GlobalAnnotationUtils.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
    }
}
