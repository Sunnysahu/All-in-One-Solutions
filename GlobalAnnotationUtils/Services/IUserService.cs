using GlobalAnnotationUtils.Common;
using GlobalAnnotationUtils.Models;

namespace GlobalAnnotationUtils.Services
{
    public interface IUserService
    {
        Task<Result<User>> GetByIdAsync(int id);
    }
}
