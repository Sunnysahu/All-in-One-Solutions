using Microsoft.AspNetCore.Mvc;
using OutBox_Pattern_with_All.Constants;
using OutBox_Pattern_with_All.Data;
using OutBox_Pattern_with_All.Entities;
using OutBox_Pattern_with_All.Models;
using System.Text.Json;

namespace OutBox_Pattern_with_All.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _db;

        public OrderController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("create-orders")]
        public async Task<IActionResult> Create(CreateOrderRequest request)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();

            var messageId = Guid.NewGuid();

            var order = new Order
            {
                Id = Guid.NewGuid(),
                ProductName = request.ProductName,
                Quantity = request.Quantity,
                CreatedAt = DateTime.Now
            };

            await _db.Orders.AddAsync(order);

            OrderCreatedEvent orderCreatedEvent = new OrderCreatedEvent
            {
                MessageId = messageId,
                OrderId = order.Id,
                ProductName = order.ProductName,
                Quantity = order.Quantity,
                CreatedAt = order.CreatedAt
            };

            OutboxMessage outbox = new OutboxMessage
            {
                Id = messageId,
                EventType = nameof(OrderCreatedEvent),
                Payload = JsonSerializer.Serialize(orderCreatedEvent),
                Status = OutboxStatus.Pending,
                RetryCount = 0,
                CreatedAt = DateTime.Now
            };

            await _db.OutboxMessages.AddAsync(outbox);

            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            return Ok(order.Id);
        }
    }
}
