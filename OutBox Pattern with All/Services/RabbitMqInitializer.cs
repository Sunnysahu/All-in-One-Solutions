using OutBox_Pattern_with_All.Constants;
using RabbitMQ.Client;

namespace OutBox_Pattern_with_All.Services
{
    public sealed class RabbitMqInitializer : IAsyncDisposable
    {
        public IChannel Channel { get; }

        public RabbitMqInitializer(RabbitMqConnection connection)
        {
            Channel = connection.Connection.CreateChannelAsync().GetAwaiter().GetResult();

            Channel.ExchangeDeclareAsync(
                exchange: RabbitMqConstants.Exchange,
                type: ExchangeType.Direct,
                durable: true
            ).GetAwaiter().GetResult();

            Channel.QueueDeclareAsync(
                queue: RabbitMqConstants.Queue,
                durable: true,
                exclusive: false,
                autoDelete: false
            ).GetAwaiter().GetResult();

            Channel.QueueBindAsync(
                queue: RabbitMqConstants.Queue,
                exchange: RabbitMqConstants.Exchange,
                routingKey: RabbitMqConstants.RoutingKey
            ).GetAwaiter().GetResult();
        }

        public async ValueTask DisposeAsync()
        {
            await Channel.DisposeAsync();
        }
    }
}
