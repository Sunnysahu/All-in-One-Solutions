using Microsoft.EntityFrameworkCore;
using OutBox_Pattern_with_All.Constants;
using OutBox_Pattern_with_All.Data;
using System.Threading.Channels;

namespace OutBox_Pattern_with_All.Services
{
    public class OutboxProcessorService : BackgroundService
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        private readonly RabbitMqPublisher _publisher;

        public OutboxProcessorService(IDbContextFactory<AppDbContext> dbFactory, RabbitMqPublisher publisher)
        {
            _dbFactory = dbFactory;
            _publisher = publisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Hello");

                await ProcessMessages(stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(5),stoppingToken);
            }
        }

        private async Task ProcessMessages(CancellationToken cancellationToken)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

            var messages =
                await db.OutboxMessages
                    .Where(x => x.Status == DbConstants.Pending)
                    .OrderBy(x => x.CreatedAt)
                    .Take(100)
                    .ToListAsync();

            foreach (var message in messages)
            {
                try
                {
                    await _publisher.PublishAsync(message.Payload);

                    message.Status = DbConstants.Processed;

                    message.ProcessedAt = DateTime.Now;
                }
                catch
                {
                    message.RetryCount++;
                }
            }

            await db.SaveChangesAsync(cancellationToken);
        }
    }
}