using JWT.Data;
using JWT.Models;
using JWT.Repository;
using Microsoft.Data.SqlClient;
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

        public async Task<User?> GetUserAsync(string username, string password, CancellationToken cancellationToken)
        {

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _context.Database.ExecuteSqlRawAsync("WAITFOR DELAY '00:00:05'", cancellationToken);
            }
            catch (SqlException)
            {
                Console.WriteLine("DB Query Cancelled");
                return null;
            }
            return await _context.Users.FirstOrDefaultAsync(x => x.Username == username && x.Password == password, cancellationToken);

            /*
             *return await _context.Users.FromSqlRaw("WAITFOR DELAY '00:00:15'; SELECT * FROM Users").FirstOrDefaultAsync(x => x.Username == username && x.Password == password, cancellationToken);
             */
        }
    }
}
