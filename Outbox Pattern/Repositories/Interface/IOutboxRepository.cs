using Outbox_Pattern.Models;

namespace Outbox_Pattern.Repositories
{
    public interface IOutboxRepository
    {
        Task AddAsync(OutboxMessage message);

        Task<List<OutboxMessage>> GetUnprocessedMessagesAsync();

        Task MarkAsProcessedAsync(Guid id);
    }
}
