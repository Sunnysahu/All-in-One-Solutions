using Microsoft.EntityFrameworkCore;
using OutBox_Pattern_with_All.Constants;
using OutBox_Pattern_with_All.Data;
using OutBox_Pattern_with_All.Models;
using System.Text.Json;

namespace OutBox_Pattern_with_All.Services
{
    public class OutboxProcessorService : BackgroundService
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        private readonly RabbitMqPublisher _publisher;
        private readonly OutboxMessageLeaseService _leaseService;

        public OutboxProcessorService(
            IDbContextFactory<AppDbContext> dbFactory, 
            RabbitMqPublisher publisher,
            OutboxMessageLeaseService leaseService
        )
        {
            _dbFactory = dbFactory;
            _publisher = publisher;
            _leaseService = leaseService;
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
            //await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

            //var messages = await db.OutboxMessages
            //    .Where(x => x.Status == DbConstants.Pending)
            //    .OrderBy(x => x.CreatedAt)
            //    .Take(100)
            //    .ToListAsync(cancellationToken);

            //var messages = await _leaseService.AcquireMessagesAsync();

            //db.OutboxMessages.Attach(message);

            var successIds = new List<Guid>();

            var failedIds = new List<Guid>();

            var messages = await _leaseService.AcquireMessagesAsync();

            foreach (var message in messages)
            {
                try
                {
                    await _publisher.PublishAsync(message.Payload);

                    successIds.Add(message.Id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    failedIds.Add(message.Id);
                }
            }

            await _leaseService.BulkMarkProcessedAsync(successIds);

            await _leaseService.BulkIncrementRetryAsync(failedIds);

            //foreach (var message in messages)
            //{
            //    try
            //    {
            //        Console.WriteLine($"Publishing Outbox Message: {message.Id}");
            //        await _publisher.PublishAsync(message.Payload);
            //        Console.WriteLine($"Published Outbox Message: {message.Id}");

            //        db.OutboxMessages.Attach(message);

            //        message.Status = DbConstants.Processed;

            //        message.ProcessedAt = DateTime.Now;

            //        message.LockedBy = null;

            //        message.LockedAt = null;
            //    }
            //    catch
            //    {
            //        db.OutboxMessages.Attach(message);

            //        message.RetryCount++;

            //        message.LockedBy = null;
            //        message.LockedAt = null;
            //    }
            //}

            //await db.SaveChangesAsync(cancellationToken);
        }
    }
}