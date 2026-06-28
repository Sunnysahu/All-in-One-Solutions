using OutBox_Pattern_with_All.Constants;
using RabbitMQ.Client;
using System.Text;

namespace OutBox_Pattern_with_All.Services
{
    public sealed class RabbitMqPublisher : IAsyncDisposable
    {
        private readonly IChannel _channel; // IChannel is NOT thread-safe.

        private readonly SemaphoreSlim _publishLock = new(1, 1);

        public RabbitMqPublisher(RabbitMqConnection connection)
        {
            _channel = connection.Connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        public async Task PublishAsync(string payload)
        {
            byte[] body = Encoding.UTF8.GetBytes(payload);

            var properties = new BasicProperties
            {
                Persistent = true
            };

            await _publishLock.WaitAsync();

            try
            {
                await _channel.BasicPublishAsync(
                    exchange: RabbitMqConstants.Exchange,
                    routingKey: RabbitMqConstants.RoutingKey,
                    mandatory: true,
                    basicProperties: properties,
                    body: body
                );
            }
            finally
            {
                _publishLock.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _channel.DisposeAsync();

            _publishLock.Dispose();
        }
    }
}
