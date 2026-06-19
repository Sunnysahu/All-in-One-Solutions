using System.ComponentModel.DataAnnotations;

namespace Outbox_Pattern.Models
{
    public class OutboxMessage
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string EventType { get; set; } = string.Empty;

        [Required]
        public string Payload { get; set; } = string.Empty;

        public bool IsProcessed { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
