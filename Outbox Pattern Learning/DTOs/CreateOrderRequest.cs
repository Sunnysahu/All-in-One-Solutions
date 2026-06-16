using System.ComponentModel.DataAnnotations;

namespace Outbox_Pattern_Learning.DTOs
{
    public sealed class CreateOrderRequest
    {
        [Required]
        [MaxLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(typeof(decimal), "0.01", "999999999999")]
        public decimal Price { get; set; }
    }
}
