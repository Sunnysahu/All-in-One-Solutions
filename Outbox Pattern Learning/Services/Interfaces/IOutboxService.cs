using Outbox_Pattern_Learning.DTOs;
using Outbox_Pattern_Learning.Models;

namespace Outbox_Pattern_Learning.Services.Interfaces
{
    // IOutboxService defines the business contract for the Transactional Outbox workflow.
    public interface IOutboxService
    {
        Task ProcessPendingMessagesAsync(int batchSize, CancellationToken cancellationToken = default);

        Task PublishMessageAsync(OutboxMessage message, CancellationToken cancellationToken = default);

        DateTime CalculateNextRetryUtc(int retryCount);

        Task<StatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken = default);

        Task<List<OutboxDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task ReplayDeadLetterAsync(Guid messageId, CancellationToken cancellationToken = default);

    }
}
