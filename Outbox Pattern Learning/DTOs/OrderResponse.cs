using Outbox_Pattern_Learning.Models;

namespace Outbox_Pattern_Learning.DTOs
{
    public sealed class OrderResponse
    {
        public Guid OrderId { get; set; }

        public string OrderNumber { get; set; } = string.Empty; // Human-readable order number.

        public Guid CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        //private static OrderResponse Map(Order order)
        //{
        //    return new OrderResponse
        //    {
        //        OrderId = order.Id,
        //        OrderNumber = order.OrderNumber,
        //        CustomerName = order.CustomerName,
        //        TotalAmount = order.TotalAmount,
        //        CreatedAt = order.CreatedAt
        //    };
        //}
    }
}
