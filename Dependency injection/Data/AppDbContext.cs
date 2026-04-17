using Dependency_injection.Models;
using Microsoft.EntityFrameworkCore;

namespace Dependency_injection.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
    }

}
