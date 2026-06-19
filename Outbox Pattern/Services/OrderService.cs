using Outbox_Pattern.Common;
using Outbox_Pattern.Data;
using Outbox_Pattern.DTOs;
using Outbox_Pattern.Errors;
using Outbox_Pattern.Models;
using Outbox_Pattern.Repositories;
using Outbox_Pattern.Repositories.Interface;
using Outbox_Pattern.Services.Interface;
using System.Text.Json;

namespace Outbox_Pattern.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderRepository _orderRepository;
        private readonly IOutboxRepository _outboxRepository;

        public OrderService(
            ApplicationDbContext context,
            IOrderRepository orderRepository,
            IOutboxRepository outboxRepository)
        {
            _context = context;
            _orderRepository = orderRepository;
            _outboxRepository = outboxRepository;
        }

        public async Task<Result<string>> CreateOrderAsync(CreateOrderRequest request)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                if (string.IsNullOrEmpty(request.ProductName) || string.IsNullOrWhiteSpace(request.ProductName) || request.Price <= 0)
                {
                    return Result<string>.Failure(OrderErrors.NotFound);
                }
                    var order = new Order
                {
                    ProductName = request.ProductName,
                    Price = request.Price,
                    CreatedAt = DateTime.Now
                };

                await _orderRepository.AddAsync(order);

                var outboxMessage = new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    EventType = "OrderCreated",
                    Payload = JsonSerializer.Serialize(order),
                    IsProcessed = false,
                    CreatedAt = DateTime.Now
                };

                await _outboxRepository.AddAsync(outboxMessage);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Result<string>.Success(OrderResult.Success);

                //await Task.Delay(5000);
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllAsync();
        }
    }
}
