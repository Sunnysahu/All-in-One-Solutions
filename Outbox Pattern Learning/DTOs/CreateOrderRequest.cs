using System.ComponentModel.DataAnnotations;

namespace Outbox_Pattern_Learning.DTOs
{
    public sealed class CreateOrderRequest
    {
        [Required(ErrorMessage = "CustomerId is required.")]
        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "CustomerName is required.")]
        [MaxLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ProductId is required.")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "ProductName is required.")]
        [MaxLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }

        [Range(typeof(decimal), "0.01", "999999999999.99", ErrorMessage = "UnitPrice must be greater than zero.")]
        public decimal UnitPrice { get; set; }
    }
}
