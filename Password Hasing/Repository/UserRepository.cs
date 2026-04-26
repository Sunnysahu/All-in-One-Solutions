using Microsoft.EntityFrameworkCore;
using Password_Hasing.Data;
using Password_Hasing.Models;

namespace Password_Hasing.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email);
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateHash(string hash, int id)
        {

            // Cleanest and Safest
            /*
            var result = await _context.Users.FindAsync(id);
            if (result == null)
            {
                return false;
            }

            result.PasswordHash = hash;
            await _context.SaveChangesAsync();
            */

            // Classical Approach
            /*
            var result = await _context.Users.FindAsync(id);
            if (result == null) return false;
            result.PasswordHash = hash;
            _context.Users.Update(result);
            await _context.SaveChangesAsync();

            */

            // performance optimization: No database read before update and only updates PasswordHash Column

            return await _context.Users.Where(x => x.Id == id).ExecuteUpdateAsync(setters => setters.SetProperty(x => x.PasswordHash, hash)) > 0;
        }
    }
}
