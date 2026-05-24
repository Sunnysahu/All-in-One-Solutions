namespace MassTransit_Setup.Models
{
    public class OrderCreatedEvent
    {
        public Guid OrderId { get; set; }

        public string ProductName { get; set; } = default!;

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
