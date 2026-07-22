using Microsoft.EntityFrameworkCore;
using PostgresSQL_CRUD.Models;

namespace PostgresSQL_CRUD.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Employee> Employees => Set<Employee>();
    }
}
