using Microsoft.EntityFrameworkCore;
using Outbox_Pattern_Learning.Data;
using Outbox_Pattern_Learning.Models;
using Outbox_Pattern_Learning.Repositories.Interfaces;

namespace Outbox_Pattern_Learning.Repositories
{
    // This repository is responsible for persisting and querying processed message records.
    public sealed class ProcessedMessageRepository : IProcessedMessageRepository
    {
        private readonly AppDbContext _context;

        public ProcessedMessageRepository(AppDbContext context) => _context = context;

        public async Task<bool> ExistsAsync(Guid messageId,
            CancellationToken cancellationToken = default)
        {
            return await _context.ProcessedMessages
                .AsNoTracking().AnyAsync(x => x.MessageId == messageId,cancellationToken);
        }

        public async Task<ProcessedMessage?> GetByMessageIdAsync(Guid messageId,
            CancellationToken cancellationToken = default)
        {
            return await _context.ProcessedMessages.AsNoTracking()
                .FirstOrDefaultAsync(x => x.MessageId == messageId, cancellationToken);
        }

        public async Task CreateAsync(ProcessedMessage processedMessage,
            CancellationToken cancellationToken = default)
        {
            await _context.ProcessedMessages.AddAsync(processedMessage, cancellationToken);
        }

        public async Task<List<ProcessedMessage>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.ProcessedMessages
                .AsNoTracking()
                .OrderByDescending(x => x.ProcessedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid messageId,CancellationToken cancellationToken = default)
        {
            var entity = await _context.ProcessedMessages
                .FirstOrDefaultAsync(x => x.MessageId == messageId, cancellationToken);

            if (entity is null) return;

            _context.ProcessedMessages.Remove(entity);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

    }
}
