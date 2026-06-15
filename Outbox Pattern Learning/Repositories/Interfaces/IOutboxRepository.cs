using Outbox_Pattern_Learning.Models;

namespace Outbox_Pattern_Learning.Repositories.Interfaces
{
    public interface IOutboxRepository
    {    
        Task CreateAsync(OutboxMessage message, CancellationToken cancellationToken = default);

        Task<OutboxMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<OutboxMessage?> GetByMessageIdAsync(Guid messageId, CancellationToken cancellationToken = default);    
        Task<List<OutboxMessage>> GetPendingMessagesAsync(int batchSize, CancellationToken cancellationToken = default);

        Task<bool> AcquireLockAsync(Guid messageId, string lockedBy, DateTime lockUntilUtc,
            CancellationToken cancellationToken = default);

        Task ReleaseLockAsync(Guid messageId, CancellationToken cancellationToken = default);

        Task MarkPublishedAsync(Guid messageId, DateTime publishedAtUtc, CancellationToken cancellationToken = default);

        Task IncrementRetryAsync(Guid messageId, int retryCount, DateTime nextRetryAtUtc, string lastError, CancellationToken cancellationToken = default);

        Task MoveToDeadLetterAsync(Guid messageId, string lastError, CancellationToken cancellationToken = default);
        Task<List<OutboxMessage>> GetAllAsync(CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
