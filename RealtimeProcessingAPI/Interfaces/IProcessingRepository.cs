using RealtimeProcessingAPI.Entities;

namespace RealtimeProcessingAPI.Interfaces
{
    public interface IProcessingRepository
    {
        Task SaveLogAsync(ProcessingLog log, CancellationToken cancellationToken);
    }
}
