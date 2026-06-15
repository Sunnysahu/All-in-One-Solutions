using Outbox_Pattern_Learning.Models;

namespace Outbox_Pattern_Learning.Services.Interfaces
{
    public interface IRabbitMqPublisher : IAsyncDisposable
    {
        Task<bool> PublishAsync(OutboxMessage message, CancellationToken cancellationToken = default);

        bool IsConnected { get; }

        Task ConnectAsync(CancellationToken cancellationToken = default);

        Task DisconnectAsync(CancellationToken cancellationToken = default);
    }
}
