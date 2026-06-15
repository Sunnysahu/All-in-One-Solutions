using Microsoft.Extensions.Options;
using Outbox_Pattern_Learning.Configuration;
using Outbox_Pattern_Learning.Services.Interfaces;

namespace Outbox_Pattern_Learning.BackgroundServices
{
    // This class is the engine that drives the Transactional Outbox Pattern.
    public sealed class OutboxPublisherBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<OutboxPublisherBackgroundService> _logger;
        private readonly OutboxOptions _options;

        public OutboxPublisherBackgroundService(
            IServiceScopeFactory serviceScopeFactory,
            IOptions<OutboxOptions> options,
            ILogger<OutboxPublisherBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox Publisher Background Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope =
                        _serviceScopeFactory.CreateScope();

                    var outboxService =
                        scope.ServiceProvider.GetRequiredService<IOutboxService>();

                    await outboxService.ProcessPendingMessagesAsync(
                        _options.BatchSize,
                        stoppingToken
                    );
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Outbox worker cancellation requested.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Unhandled exception while processing Outbox messages."
                    );
                }

                try
                {
                    await Task.Delay(
                        TimeSpan.FromSeconds(_options.PollingIntervalSeconds),
                        stoppingToken
                    );
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            _logger.LogInformation("Outbox Publisher Background Service stopped.");
        }
    }
}
