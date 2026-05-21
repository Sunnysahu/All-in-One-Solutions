using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Hook.Models
{
    [Table("Payments")]
    public class Payment
    {
        public int Id { get; set; }
        public string OrderId { get; set; } = default;
        public string PaymentId { get; set; } = default;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = default;
        public string Status { get; set; } = default;
        public DateTime CreatedAt { get; set; }
    }
}
