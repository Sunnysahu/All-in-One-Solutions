using GlobalExceptionAndLogging.Entities;
using Microsoft.EntityFrameworkCore;

namespace GlobalExceptionAndLogging.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
}