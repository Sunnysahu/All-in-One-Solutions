namespace OutBox_Pattern_with_All.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }

        public string EventType { get; set; }

        public string Payload { get; set; }

        public int Status { get; set; }

        public int RetryCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ProcessedAt { get; set; }
    }
}
