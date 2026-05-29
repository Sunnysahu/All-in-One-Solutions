using MassTransit;
using MassTransit_Using_RabbitMQ.Contracts.Events;
using MassTransit_Using_RabbitMQ.Data;
using MassTransit_Using_RabbitMQ.Entities;

namespace MassTransit_Using_RabbitMQ.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(ApplicationDbContext context, ILogger<OrderCreatedConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var message = context.Message;

             // await Task.Delay(20000);

            _logger.LogInformation($"Processing OrderId : {message.OrderId}");

            var order = new Order
            {
                ProductName = message.ProductName,
                Quantity = message.Quantity,
                Price = message.Price,
                Status = "Processed",
                CreatedAt = DateTime.Now
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Order Placed Successfully");
        }
    }
}
