using Web_Hook.Models;

namespace Web_Hook.Services.Interfaces
{
    public interface IWebhookQueue
    {
        ValueTask QueueAsync(WebhookEvent webhookEvent);
        ValueTask<WebhookEvent> DequeueAsync(CancellationToken cancellationToken);
    }
}
