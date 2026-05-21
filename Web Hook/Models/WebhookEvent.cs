namespace Web_Hook.Models
{
    public class WebhookEvent
    {
        public long Id { get; set; }
        public string EventId { get; set; } = default;
        public string EventType { get; set; } = default;
        public string Payload { get; set; } = default;
        public string Signature { get; set; } = default;
        public bool IsProcessed { get; set; }
        public int RetryCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
