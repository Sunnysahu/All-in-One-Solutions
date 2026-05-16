using RealtimeProcessingAPI.DTOs;
using RealtimeProcessingAPI.Entities;
using RealtimeProcessingAPI.Interfaces;
using ProcessingLog = RealtimeProcessingAPI.Entities.ProcessingLog;

namespace RealtimeProcessingAPI.Services
{
    public class ProcessingService : IProcessingService
    {
        private readonly IProcessingRepository _repository;

        public ProcessingService(IProcessingRepository repository) => _repository = repository;        

        public async IAsyncEnumerable<ProgressDto> ProcessAsync([System.Runtime.CompilerServices.EnumeratorCancellation]CancellationToken cancellationToken)
        {
            string[] steps = {"Starting Process", "Validating Data", "Processing Records", "Saving To Database", "Completed" };

            for (int i = 0; i < steps.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(2000, cancellationToken);

                var log = new ProcessingLog
                {
                    Message = steps[i],
                    Status = "Running",
                    StepNumber = i + 1,
                    CreatedAt = DateTime.UtcNow
                };

                await _repository.SaveLogAsync(log, cancellationToken);

                yield return new ProgressDto
                {
                    Step = i + 1,
                    Message = steps[i]
                };
            }
        }
    }
}