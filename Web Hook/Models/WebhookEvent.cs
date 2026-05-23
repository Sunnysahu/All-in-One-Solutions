namespace Web_Hook.Models
{
    public class WebhookEvent
    {
        public long Id { get; set; }
        public string EventId { get; set; }
        public string EventType { get; set; } 
        public string Payload { get; set; } 
        public string Signature { get; set; }
        public bool IsProcessed { get; set; }
        public int RetryCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
