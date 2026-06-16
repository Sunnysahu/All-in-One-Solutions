using Outbox_Pattern_Learning.Models;

namespace Outbox_Pattern_Learning.DTOs
{
    public sealed class OrderResponse
    {
        public Guid Id { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal Price { get; set; }

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
