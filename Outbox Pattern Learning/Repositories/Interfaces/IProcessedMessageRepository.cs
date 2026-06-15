using Outbox_Pattern_Learning.Models;

namespace Outbox_Pattern_Learning.Repositories.Interfaces
{
    public interface IProcessedMessageRepository
    {
        Task<bool> ExistsAsync(Guid messageId, CancellationToken cancellationToken = default);

        Task<ProcessedMessage?> GetByMessageIdAsync(Guid messageId, CancellationToken cancellationToken = default);

        Task CreateAsync(ProcessedMessage processedMessage, CancellationToken cancellationToken = default);

        Task<List<ProcessedMessage>> GetAllAsync(CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid messageId, CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
