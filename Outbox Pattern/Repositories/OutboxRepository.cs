using Microsoft.EntityFrameworkCore;
using Outbox_Pattern.Data;
using Outbox_Pattern.Models;

namespace Outbox_Pattern.Repositories
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly ApplicationDbContext _context;

        public OutboxRepository(ApplicationDbContext context) =>  _context = context;


        public async Task AddAsync(OutboxMessage message)
        {
            await _context.OutboxMessages.AddAsync(message);
        }

        public async Task<List<OutboxMessage>> GetUnprocessedMessagesAsync()
        {
            return await _context.OutboxMessages
                .Where(x => !x.IsProcessed)
                .ToListAsync();
        }

        public async Task MarkAsProcessedAsync(Guid id)
        {
            var message = await _context.OutboxMessages.FindAsync(id);

            if (message == null)
                return;

            message.IsProcessed = true;
        }
    }
}
