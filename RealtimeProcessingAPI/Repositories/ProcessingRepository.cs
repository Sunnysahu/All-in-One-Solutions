using RealtimeProcessingAPI.Data;
using RealtimeProcessingAPI.Entities;
using RealtimeProcessingAPI.Interfaces;

namespace RealtimeProcessingAPI.Repositories
{
    public class ProcessingRepository : IProcessingRepository
    {
        private readonly AppDbContext _context;

        public ProcessingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveLogAsync(ProcessingLog log, CancellationToken cancellationToken)
        {
            await _context.ProcessingLogs.AddAsync(log, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
