using Microsoft.EntityFrameworkCore;
using RealtimeProcessingAPI.Entities;

namespace RealtimeProcessingAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        public DbSet<ProcessingLog> ProcessingLogs => Set<ProcessingLog>();
    }
}
