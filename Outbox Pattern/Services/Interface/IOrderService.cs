using Outbox_Pattern.Common;
using Outbox_Pattern.DTOs;
using Outbox_Pattern.Errors;
using Outbox_Pattern.Models;

namespace Outbox_Pattern.Services.Interface
{
    public interface IOrderService
    {
        Task<Result<string>> CreateOrderAsync(CreateOrderRequest request);

        Task<List<Order>> GetAllOrdersAsync();
    }
}
