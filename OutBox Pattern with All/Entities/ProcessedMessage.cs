using System.ComponentModel.DataAnnotations;

namespace OutBox_Pattern_with_All.Entities
{
    public class ProcessedMessage
    {
        [Key]
        public Guid MessageId { get; set; }

        public DateTime ProcessedAt { get; set; }
    }
}
