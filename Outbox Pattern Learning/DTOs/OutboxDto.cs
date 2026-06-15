namespace Outbox_Pattern_Learning.DTOs
{
    public sealed class OutboxDto
    {
        public Guid Id { get; set; } // Internal Outbox record identifier.

        public Guid MessageId { get; set; }

        public Guid CorrelationId { get; set; } // Correlation identifier shared across related events.

        public Guid OrderId { get; set; } // Related order identifier.

        public string EventType { get; set; } = string.Empty; //Event name -> Example: OrderCreated.

        public string Status { get; set; } = string.Empty; // Current Outbox processing status.

        public int RetryCount { get; set; } // Number of publish attempts.

        public DateTime? LastAttemptAt { get; set; }

        public DateTime? NextRetryAt { get; set; }

        public DateTime? PublishedAt { get; set; }

        public string? LastError { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
