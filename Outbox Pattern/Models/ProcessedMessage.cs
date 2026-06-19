using System.ComponentModel.DataAnnotations;

namespace Outbox_Pattern.Models
{
    public class ProcessedMessage
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime ProcessedAt { get; set; }
    }
}
