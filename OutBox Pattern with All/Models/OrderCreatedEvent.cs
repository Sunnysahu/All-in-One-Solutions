namespace OutBox_Pattern_with_All.Models
{
    public class OrderCreatedEvent
    {
        public Guid OrderId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
