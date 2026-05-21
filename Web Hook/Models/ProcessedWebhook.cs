using Microsoft.EntityFrameworkCore;

namespace Web_Hook.Models
{
    [Index(nameof(EventId), IsUnique = true)]
    public class ProcessedWebhook
    {
        public long Id { get; set; }
        public string EventId { get; set; } = default;
        public DateTime ProcessedAt { get; set; }
    }
}
