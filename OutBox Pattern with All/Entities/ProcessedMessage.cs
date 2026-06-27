namespace OutBox_Pattern_with_All.Entities
{
    public class ProcessedMessage
    {
        public Guid MessageId { get; set; }

        public DateTime ProcessedAt { get; set; }
    }
}
