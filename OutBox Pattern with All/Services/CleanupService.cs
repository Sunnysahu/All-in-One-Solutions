using Microsoft.EntityFrameworkCore;
using OutBox_Pattern_with_All.Constants;
using OutBox_Pattern_with_All.Data;

namespace OutBox_Pattern_with_All.Services
{
    public class CleanupService : BackgroundService
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        public CleanupService(IDbContextFactory<AppDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CleanupAsync(stoppingToken);

                 await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                //await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Testing Purpose
            }
        }

        private async Task CleanupAsync(CancellationToken cancellationToken)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

            var outboxDate = DateTime.UtcNow.AddDays(-7);

            var processedDate = DateTime.UtcNow.AddDays(-30);

            await db.OutboxMessages.Where(x =>
                x.Status == DbConstants.Processed &&
                x.ProcessedAt < outboxDate
            ).ExecuteDeleteAsync(cancellationToken);

            await db.ProcessedMessages
                .Where(x => x.ProcessedAt < processedDate)
                .ExecuteDeleteAsync(cancellationToken);

            Console.WriteLine("Cleanup completed");
        }
    }
}
