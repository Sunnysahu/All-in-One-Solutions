using JWT.Data;
using JWT.Models;
using JWT.Repository;
using Microsoft.EntityFrameworkCore;

namespace JWT.Services
{
    public class UserRepository : IUserRepository
    {
        
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserAsync(string username, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Username == username && x.Password == password);
        }
    }
}
