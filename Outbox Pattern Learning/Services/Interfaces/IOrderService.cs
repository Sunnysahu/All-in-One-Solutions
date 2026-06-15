using Outbox_Pattern_Learning.DTOs;

namespace Outbox_Pattern_Learning.Services.Interfaces
{
    // This interface defines the business contract for order operations.
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);

        Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<List<OrderResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
