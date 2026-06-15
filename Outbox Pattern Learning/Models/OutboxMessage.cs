using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Outbox_Pattern_Learning.Models
{
    public sealed class OutboxMessage
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid MessageId { get; set; }

        [Required]
        public Guid CorrelationId { get; set; }

        [Required]
        public Guid OrderId { get; set; }

        [Required]
        [MaxLength(200)]
        public string EventType { get; set; } = string.Empty;

        [Required]
        public string Payload { get; set; } = string.Empty;

        public OutboxStatus Status { get; set; }

        public int RetryCount { get; set; }

        public string? LockedBy { get; set; }

        public DateTime? LockedUntil { get; set; }

        public DateTime? LastAttemptAt { get; set; }

        public DateTime? NextRetryAt { get; set; }

        public DateTime? PublishedAt { get; set; }

        public string? LastError { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
