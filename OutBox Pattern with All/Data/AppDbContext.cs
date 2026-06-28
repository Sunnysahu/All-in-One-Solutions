using Microsoft.EntityFrameworkCore;
using OutBox_Pattern_with_All.Entities;
using System.Collections.Generic;

namespace OutBox_Pattern_with_All.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders => Set<Order>();

        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        public DbSet<ProcessedMessage> ProcessedMessages { get; set; }
    }
}
