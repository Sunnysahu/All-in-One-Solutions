using Microsoft.EntityFrameworkCore;
using Web_Hook.Models;

namespace Web_Hook.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Payment> payment => Set<Payment>();
        public DbSet<WebhookEvent> WebhookEvents => Set<WebhookEvent>();
        public DbSet<ProcessedWebhook> ProcessedWebhooks => Set<ProcessedWebhook>();
    }
}
