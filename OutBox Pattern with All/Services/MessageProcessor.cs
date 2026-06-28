using Microsoft.EntityFrameworkCore;
using OutBox_Pattern_with_All.Data;
using OutBox_Pattern_with_All.Entities;
using OutBox_Pattern_with_All.Models;
using System.Text.Json;

namespace OutBox_Pattern_with_All.Services
{
    public class MessageProcessor
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        public MessageProcessor(IDbContextFactory<AppDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task ProcessAsync(string payload)
        {
            var orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(payload)!;

            await using var db =
                await _dbFactory.CreateDbContextAsync();

            bool alreadyProcessed =
                await db.ProcessedMessages.AnyAsync(x => x.MessageId == orderCreatedEvent.MessageId);

            if (alreadyProcessed)
            {
                Console.WriteLine($"Duplicate Message : {orderCreatedEvent.MessageId}");

                return;
            }

            Console.WriteLine("------------------------------------");
            Console.WriteLine($"Order Id     : {orderCreatedEvent.OrderId}");
            Console.WriteLine($"Product Name : {orderCreatedEvent.ProductName}");
            Console.WriteLine($"Quantity     : {orderCreatedEvent.Quantity}");
            Console.WriteLine("------------------------------------");

            await db.ProcessedMessages.AddAsync(
            new ProcessedMessage
            {
                MessageId = orderCreatedEvent.MessageId,
                ProcessedAt = DateTime.UtcNow
            });

            await db.SaveChangesAsync();
        }
    }
}
