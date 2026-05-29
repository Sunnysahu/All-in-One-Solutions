using System.ComponentModel.DataAnnotations.Schema;

namespace MassTransit_Using_RabbitMQ.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; }
    }
}
