using Microsoft.EntityFrameworkCore;
using Outbox_Pattern.Data;
using Outbox_Pattern.Models;
using Outbox_Pattern.Repositories.Interface;

namespace Outbox_Pattern.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context) => _context = context;

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders.ToListAsync();
        }
    }
}
