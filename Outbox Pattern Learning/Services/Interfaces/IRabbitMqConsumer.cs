namespace Outbox_Pattern_Learning.Services.Interfaces
{
    public interface IRabbitMqConsumer : IAsyncDisposable
    {
        Task StartAsync(CancellationToken cancellationToken = default);

        Task StopAsync(CancellationToken cancellationToken = default);

        bool IsRunning { get; }

        bool IsConnected { get; }
    }
}
