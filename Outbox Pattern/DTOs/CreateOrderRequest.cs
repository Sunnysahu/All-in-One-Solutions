namespace Outbox_Pattern.DTOs
{
    public class CreateOrderRequest
    {
        public string ProductName { get; set; } = string.Empty;

        public decimal Price { get; set; }
    }
}
