using RealtimeProcessingAPI.DTOs;

namespace RealtimeProcessingAPI.Interfaces
{
    public interface IProcessingService
    {
        IAsyncEnumerable<ProgressDto> ProcessAsync(CancellationToken cancellationToken);
    }
}
