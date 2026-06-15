using Outbox_Pattern_Learning.DTOs;
using Outbox_Pattern_Learning.Models;
using Outbox_Pattern_Learning.Repositories.Interfaces;
using Outbox_Pattern_Learning.Services.Interfaces;

namespace Outbox_Pattern_Learning.Services
{

    // OutboxService is the core business engine of the Transactional Outbox Pattern.
    public class OutboxService : IOutboxService
    {
        private readonly IOutboxRepository _outboxRepository;
        private readonly IRabbitMqPublisher _rabbitMqPublisher;
        private readonly ILogger<OutboxService> _logger;

        private readonly string _workerId = Environment.MachineName;

        public OutboxService(IOutboxRepository outboxRepository, IRabbitMqPublisher rabbitMqPublisher, ILogger<OutboxService> logger)
        {
            _outboxRepository = outboxRepository;
            _rabbitMqPublisher = rabbitMqPublisher;
            _logger = logger;
        }

        public async Task ProcessPendingMessagesAsync(int batchSize,
            CancellationToken cancellationToken = default)
        {
            var messages = await _outboxRepository.GetPendingMessagesAsync(
                batchSize,
                cancellationToken);

            _logger.LogInformation(
                "Found {Count} pending outbox messages.",
                messages.Count
            );

            foreach (var message in messages)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var lockAcquired = await _outboxRepository.AcquireLockAsync(
                        message.Id,
                        _workerId,
                        DateTime.UtcNow.AddMinutes(1),
                        cancellationToken
                );

                if (!lockAcquired)
                {
                    continue;
                }

                await PublishMessageAsync(
                    message,
                    cancellationToken
                );
            }
        }

        public async Task PublishMessageAsync(
        OutboxMessage message,
        CancellationToken cancellationToken = default)
        {
            try
            {
                if (!_rabbitMqPublisher.IsConnected)
                {
                    await _rabbitMqPublisher.ConnectAsync(cancellationToken);
                }

                var published =await _rabbitMqPublisher.PublishAsync(
                        message,
                        cancellationToken
                );

                if (published)
                {
                    await _outboxRepository.MarkPublishedAsync(
                        message.Id,
                        DateTime.UtcNow,
                        cancellationToken
                    );

                    _logger.LogInformation(
                        "Published message {MessageId}.",
                        message.MessageId
                    );

                    return;
                }

                throw new Exception("RabbitMQ returned negative confirmation.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Publishing failed for message {MessageId}.",
                    message.MessageId
                );

                var retryCount = message.RetryCount + 1;

                if (retryCount >= 8)
                {
                    await _outboxRepository.MoveToDeadLetterAsync(
                        message.Id,
                        ex.Message,
                        cancellationToken
                    );

                    _logger.LogWarning(
                        "Message {MessageId} moved to DeadLetter.",
                        message.MessageId
                    );

                    return;
                }

                var nextRetry =
                    CalculateNextRetryUtc(retryCount);

                await _outboxRepository.IncrementRetryAsync(
                    message.Id,
                    retryCount,
                    nextRetry,
                    ex.Message,
                    cancellationToken
                );

                _logger.LogWarning(
                    "Retry {Retry} scheduled for message {MessageId} at {NextRetry}.",
                    retryCount,
                    message.MessageId,
                    nextRetry
                );
            }
        }

        public DateTime CalculateNextRetryUtc(
        int retryCount)
        {
            var delay = retryCount switch
            {
                1 => TimeSpan.FromSeconds(1),
                2 => TimeSpan.FromSeconds(2),
                3 => TimeSpan.FromSeconds(4),
                4 => TimeSpan.FromSeconds(8),
                5 => TimeSpan.FromSeconds(16),
                6 => TimeSpan.FromSeconds(32),
                7 => TimeSpan.FromMinutes(1),
                _ => TimeSpan.FromMinutes(5)
            };

            return DateTime.UtcNow.Add(delay);
        }

        public async Task<List<OutboxDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var messages = await _outboxRepository.GetAllAsync(cancellationToken);

            return messages.Select(x => new OutboxDto
            {
                Id = x.Id,
                MessageId = x.MessageId,
                CorrelationId = x.CorrelationId,
                EventType = x.EventType,
                Status = x.Status.ToString(),
                RetryCount = x.RetryCount,
                CreatedAt = x.CreatedAt,
                PublishedAt = x.PublishedAt
            }).ToList();
        }

        public async Task<StatisticsDto> GetStatisticsAsync(
        CancellationToken cancellationToken = default)
        {
            var messages =
                await _outboxRepository.GetAllAsync(
                    cancellationToken);

            return new StatisticsDto
            {
                TotalMessages = messages.Count,
                PendingMessages = messages.Count(x => x.Status == OutboxStatus.Pending),
                PublishedMessages = messages.Count(x => x.Status == OutboxStatus.Published),
                FailedMessages = messages.Count(x => x.Status == OutboxStatus.Failed),
                DeadLetterMessages = messages.Count(x => x.Status == OutboxStatus.DeadLetter),
                LockedMessages = messages.Count(x => x.Status == OutboxStatus.Locked)
            };
        }

        public async Task ReplayDeadLetterAsync(Guid messageId,
        CancellationToken cancellationToken = default)
        {
            var message =
                await _outboxRepository.GetByIdAsync(messageId, cancellationToken);

            if (message is null)
            {
                throw new InvalidOperationException("Outbox message not found.");
            }

            message.Status = OutboxStatus.Pending;
            message.RetryCount = 0;
            message.LastError = null;
            message.NextRetryAt = null;
            message.LockedBy = null;
            message.LockedUntil = null;
            message.UpdatedAt = DateTime.UtcNow;

            await _outboxRepository.SaveChangesAsync(
                cancellationToken);

            _logger.LogInformation(
                "DeadLetter message {MessageId} replayed.",
                message.MessageId);
        }
    }
}
