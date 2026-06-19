using Microsoft.EntityFrameworkCore;
using Outbox_Pattern.Data;
using Outbox_Pattern.Models;

namespace Outbox_Pattern.BackgroundServices
{
    public class OutboxProcessor : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public OutboxProcessor(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var context = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                var messages = await context.OutboxMessages
                    .Where(x => !x.IsProcessed)
                    .ToListAsync();

                foreach (var message in messages)
                {
                    try
                    {
                        Console.WriteLine("--------------------------------");

                        Console.WriteLine("Publishing Event");

                        Console.WriteLine(message.Payload);

                        Console.WriteLine("--------------------------------");

                        context.ProcessedMessages.Add(new ProcessedMessage
                        {
                            Id = message.Id,
                            ProcessedAt = DateTime.Now
                        });

                        message.IsProcessed = true;
                    }
                    catch
                    {
                        Console.WriteLine("Publishing Failed");
                    }
                }

                await context.SaveChangesAsync();

                 await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
