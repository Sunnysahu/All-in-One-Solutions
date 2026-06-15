using System.ComponentModel.DataAnnotations;

namespace Outbox_Pattern_Learning.Models
{
    public class ProcessedMessage
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Unique message identifier received from RabbitMQ.
        /// Used to prevent duplicate business processing.
        /// </summary>
        
        [Required]
        public Guid MessageId { get; set; }

        /// <summary>
        /// Correlation identifier shared across related events.
        /// Useful for distributed tracing.
        /// </summary>
       
        [Required]
        public Guid CorrelationId { get; set; }

        [Required]
        [MaxLength(200)]
        public string EventType { get; set; } = string.Empty; // Event type that was processed. // Example: OrderCreated.

        public DateTime ProcessedAt { get; set; }

        [MaxLength(200)]
        public string ProcessedBy { get; set; } = string.Empty; // Worker or application instance that processed the message.
    }
}
