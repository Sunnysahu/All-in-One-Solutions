using GlobalAnnotationUtils.Models;

namespace GlobalAnnotationUtils.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users =
        [
            new User
        {
            Id = 1,
            Name = "Sunny"
        }
        ];

        public Task<User?> GetByIdAsync(int id)
        {
            return Task.FromResult(_users.FirstOrDefault(x => x.Id == id));
        }
    }
}
