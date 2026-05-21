using Web_Hook.Models;

namespace Web_Hook.Services.Interfaces
{
    public interface IWebhookService
    {
        Task<bool> IsDuplicateAsync(string eventId);
        Task SaveWebhookEventAsync(WebhookEvent webhookEvent);
        Task ProcessWebhookAsync(WebhookEvent webhookEvent);
    }
}
