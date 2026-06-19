using System.ComponentModel.DataAnnotations;

namespace Outbox_Pattern.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
