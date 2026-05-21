
using Web_Hook.Services.Interfaces;

namespace Web_Hook.BackgroundServices
{
    public class WebhookProcessorBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IWebhookQueue _queue;
        private readonly ILogger<WebhookProcessorBackgroundService> _logger;

        public WebhookProcessorBackgroundService(IServiceScopeFactory serviceScopeFactory, IWebhookQueue queue, ILogger<WebhookProcessorBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _queue = queue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Webhook Background Service Started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Waiting for webhook in queue...");
                    var webhookEvent = await _queue.DequeueAsync(stoppingToken);

                    _logger.LogInformation("Webhook dequeued. EventId: {EventId}", webhookEvent.EventId);
                    await Task.Delay(2000, stoppingToken);
                    using var scope = _serviceScopeFactory.CreateScope();

                    var webhookService = scope.ServiceProvider.GetRequiredService<IWebhookService>();

                    await webhookService.ProcessWebhookAsync(webhookEvent);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Background Processing Failed: {Message}", ex.Message);
                }
            }
        }
    }
}
