using BloomFilterDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace BloomFilterDemo.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}