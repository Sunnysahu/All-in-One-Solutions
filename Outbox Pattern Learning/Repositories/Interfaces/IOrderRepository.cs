using Outbox_Pattern_Learning.Models;

namespace Outbox_Pattern_Learning.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task CreateAsync(Order order, CancellationToken cancellationToken = default);

        Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);

        Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default);

        Task UpdateAsync(Order order, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
