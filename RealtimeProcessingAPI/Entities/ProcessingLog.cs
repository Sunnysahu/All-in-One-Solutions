namespace RealtimeProcessingAPI.Entities
{
    public class ProcessingLog
    {
        public int Id { get; init; }

        public required string Message { get; init; }

        public string? Status { get; init; }

        public int StepNumber { get; init; }

        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
}
