using Microsoft.EntityFrameworkCore;
using Outbox_Pattern.Models;

namespace Outbox_Pattern.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        public DbSet<ProcessedMessage> ProcessedMessages { get; set; }
    }
}
