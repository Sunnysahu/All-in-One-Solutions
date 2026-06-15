using Microsoft.EntityFrameworkCore;
using Outbox_Pattern_Learning.Data;
using Outbox_Pattern_Learning.Models;
using Outbox_Pattern_Learning.Repositories.Interfaces;

namespace Outbox_Pattern_Learning.Repositories
{
    /// <summary>
    /// Repository responsible for Transactional Outbox persistence.
    /// Contains database access logic only.
    /// </summary>

    // This repository encapsulates all persistence logic for the Transactional Outbox table.
    public sealed class OutboxRepository : IOutboxRepository
    {
        private readonly AppDbContext _context;

        public OutboxRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(OutboxMessage message, CancellationToken cancellationToken = default)
        {
            await _context.OutboxMessages.AddAsync(message, cancellationToken);
        }

        public async Task<OutboxMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.OutboxMessages
                .FirstOrDefaultAsync(x => x.Id == id,cancellationToken);
        }

        public async Task<OutboxMessage?> GetByMessageIdAsync(Guid messageId, CancellationToken cancellationToken = default)
        {
            return await _context.OutboxMessages
                .FirstOrDefaultAsync(x => x.MessageId == messageId, cancellationToken);
        }

        public async Task<List<OutboxMessage>> GetPendingMessagesAsync(int batchSize,
            CancellationToken cancellationToken = default)
        {
            var utcNow = DateTime.UtcNow;

            return await _context.OutboxMessages
                .Where(x =>
                (
                    x.Status == OutboxStatus.Pending ||
                    (
                        x.Status == OutboxStatus.Failed && 
                        x.NextRetryAt != null &&
                        x.NextRetryAt <= utcNow
                    )
                )
                &&
                (
                    x.LockedUntil == null || x.LockedUntil <= utcNow
                ))
                .OrderBy(x => x.CreatedAt)
                .Take(batchSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> AcquireLockAsync(Guid messageId, string lockedBy, DateTime lockUntilUtc, CancellationToken cancellationToken = default)
        {
            var message = await _context.OutboxMessages
                .FirstOrDefaultAsync(x => x.Id == messageId, cancellationToken);

            if (message == null)
            {
                return false;
            }

            if (message.LockedUntil != null && message.LockedUntil > DateTime.UtcNow)
            {
                return false;
            }

            message.Status = OutboxStatus.Locked;
            message.LockedBy = lockedBy;
            message.LockedUntil = lockUntilUtc;
            message.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task ReleaseLockAsync(
            Guid messageId,
            CancellationToken cancellationToken = default)
        {
            var message = await _context.OutboxMessages
                .FirstOrDefaultAsync(x => x.Id == messageId, cancellationToken);

            if (message == null)
            {
                return;
            }

            message.LockedBy = null;
            message.LockedUntil = null;

            if (message.Status == OutboxStatus.Locked)
            {
                message.Status = OutboxStatus.Pending;
            }

            message.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task MarkPublishedAsync(
            Guid messageId,
            DateTime publishedAtUtc,
            CancellationToken cancellationToken = default)
        {
            var message = await _context.OutboxMessages
                .FirstOrDefaultAsync(
                    x => x.Id == messageId,
                    cancellationToken);

            if (message == null)
            {
                return;
            }

            message.Status = OutboxStatus.Published;
            message.PublishedAt = publishedAtUtc;
            message.LockedBy = null;
            message.LockedUntil = null;
            message.LastAttemptAt = publishedAtUtc;
            message.UpdatedAt = publishedAtUtc;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task IncrementRetryAsync(
            Guid messageId,
            int retryCount,
            DateTime nextRetryAtUtc,
            string lastError,
            CancellationToken cancellationToken = default)
        {
            var message = await _context.OutboxMessages
                .FirstOrDefaultAsync(
                    x => x.Id == messageId,
                    cancellationToken);

            if (message == null)
            {
                return;
            }

            message.Status = OutboxStatus.Failed;
            message.RetryCount = retryCount;
            message.NextRetryAt = nextRetryAtUtc;
            message.LastAttemptAt = DateTime.UtcNow;
            message.LastError = lastError;

            message.LockedBy = null;
            message.LockedUntil = null;
            message.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task MoveToDeadLetterAsync(
            Guid messageId,
            string lastError,
            CancellationToken cancellationToken = default)
        {
            var message = await _context.OutboxMessages
                .FirstOrDefaultAsync(x => x.Id == messageId,
                    cancellationToken);

            if (message == null)
            {
                return;
            }

            message.Status = OutboxStatus.DeadLetter;
            message.LastError = lastError;
            message.LastAttemptAt = DateTime.UtcNow;

            message.LockedBy = null;
            message.LockedUntil = null;
            message.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<OutboxMessage>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.OutboxMessages
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
