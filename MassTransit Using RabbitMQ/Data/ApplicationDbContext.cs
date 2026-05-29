using MassTransit_Using_RabbitMQ.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassTransit_Using_RabbitMQ.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders => Set<Order>();
    }
}
