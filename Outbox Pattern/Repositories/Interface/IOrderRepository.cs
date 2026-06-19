using Outbox_Pattern.Models;

namespace Outbox_Pattern.Repositories.Interface
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);

        Task<List<Order>> GetAllAsync();
    }
}
