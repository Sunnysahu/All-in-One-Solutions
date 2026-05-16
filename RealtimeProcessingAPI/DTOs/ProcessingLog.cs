namespace RealtimeProcessingAPI.DTOs
{
    public class ProcessingLog
    {
        public int Id { get; set; }

        public string Message { get; set; } = string.Empty;

        public string? Status { get; set; }

        public int StepNumber { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
