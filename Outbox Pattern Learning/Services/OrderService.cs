using Outbox_Pattern_Learning.Data;
using Outbox_Pattern_Learning.DTOs;
using Outbox_Pattern_Learning.Models;
using Outbox_Pattern_Learning.Repositories.Interfaces;
using Outbox_Pattern_Learning.Services.Interfaces;
using System.Text.Json;

namespace Outbox_Pattern_Learning.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IOrderRepository _orderRepository;
        private readonly IOutboxRepository _outboxRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            AppDbContext context, 
            IOrderRepository orderRepository, 
            IOutboxRepository outboxRepository, 
            ILogger<OrderService> logger
        )
        {
            _context = context;
            _orderRepository = orderRepository;
            _outboxRepository = outboxRepository;
            _logger = logger;
        }

        public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request,
       CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Starting order creation for customer {CustomerName}.",
                request.CustomerName);

            await using var transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    CustomerName = request.CustomerName,
                    ProductName = request.ProductName,
                    Quantity = request.Quantity,
                    Price = request.Price,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _orderRepository.CreateAsync(
                    order,
                    cancellationToken);

                _logger.LogInformation(
                    "Order entity created with Id {OrderId}.",
                    order.Id);

                var eventPayload = JsonSerializer.Serialize(new
                {
                    order.Id,
                    order.OrderNumber,
                    order.CustomerName,
                    order.ProductName,
                    order.Quantity,
                    order.TotalAmount,
                    order.CreatedAt
                });

                var outboxMessage = new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    MessageId = Guid.NewGuid(),
                    CorrelationId = Guid.NewGuid(),
                    EventType = "OrderCreated",
                    Payload = eventPayload,

                    Status = OutboxStatus.Pending,

                    RetryCount = 0,

                    LockedBy = null,
                    LockedUntil = null,

                    LastAttemptAt = null,
                    PublishedAt = null,
                    NextRetryAt = null,

                    LastError = null,

                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _outboxRepository.CreateAsync(
                    outboxMessage,
                    cancellationToken
                );

                _logger.LogInformation(
                    "Outbox message created with MessageId {MessageId}.",
                    outboxMessage.MessageId
                );

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation(
                    "Transaction committed successfully for OrderId {OrderId}.",
                    order.Id
                );

                return Map(order);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                _logger.LogError(
                    ex,
                    "Transaction rolled back while creating order."
                );

                throw;
            }
        }

        public async Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetByIdAsync(id, cancellationToken);

            if (order is null)
            {
                return null;
            }

            return Map(order);
        }

        public async Task<List<OrderResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.GetAllAsync(cancellationToken);

            return orders.Select(Map).ToList();
        }

        private static OrderResponse Map(Order order)
        {
            return new OrderResponse
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                CustomerName = order.CustomerName,
                ProductName = order.ProductName,
                Quantity = order.Quantity,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt
            };
        }
    }
}
