using OutBox_Pattern_with_All.Constants;
using RabbitMQ.Client;

namespace OutBox_Pattern_with_All.Services
{
    public sealed class RabbitMqTopology
    {
        public RabbitMqTopology(RabbitMqConnection rabbitMqConnection)
        {
            InitializeAsync(rabbitMqConnection.Connection)
                .GetAwaiter()
                .GetResult();
        }

        private static async Task InitializeAsync(IConnection connection)
        {
            await using var channel = await connection.CreateChannelAsync();

            // Exchange
            await channel.ExchangeDeclareAsync(
                exchange: RabbitMqConstants.Exchange,
                type: ExchangeType.Direct,
                durable: true
            );

            // ===========================
            // Dead Letter Queue
            // ===========================

            await channel.QueueDeclareAsync(
                queue: RabbitMqConstants.DeadLetterQueue,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            await channel.QueueBindAsync(
                queue: RabbitMqConstants.DeadLetterQueue,
                exchange: RabbitMqConstants.Exchange,
                routingKey: RabbitMqConstants.DeadLetterRoutingKey
            );

            // ===========================
            // Retry Queue
            // ===========================

            var retryArguments = new Dictionary<string, object?>
            {
                ["x-message-ttl"] = RabbitMqConstants.RetryDelayMilliseconds,

                ["x-dead-letter-exchange"] = RabbitMqConstants.Exchange,

                ["x-dead-letter-routing-key"] = RabbitMqConstants.RoutingKey
            };

            await channel.QueueDeclareAsync(
                queue: RabbitMqConstants.RetryQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: retryArguments
            );

            await channel.QueueBindAsync(
                queue: RabbitMqConstants.RetryQueue,
                exchange: RabbitMqConstants.Exchange,
                routingKey: RabbitMqConstants.RetryRoutingKey
            );

            // ===========================
            // Main Queue
            // ===========================

            var mainArguments = new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] = RabbitMqConstants.Exchange,

                ["x-dead-letter-routing-key"] = RabbitMqConstants.RetryRoutingKey
            };

            await channel.QueueDeclareAsync(
                queue: RabbitMqConstants.Queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: mainArguments
            );

            await channel.QueueBindAsync(
                queue: RabbitMqConstants.Queue,
                exchange: RabbitMqConstants.Exchange,
                routingKey: RabbitMqConstants.RoutingKey
            );
        }
    }
}

//if you are using rabbit mq below 7.x.x 

//then if channel closes, need to re opened

//use this code

//```
//using RabbitMQ.Client;

//namespace OutBox_Pattern_with_All.Services
//{
//    public sealed class RabbitMqPublisherChannel : IAsyncDisposable
//    {
//        private readonly RabbitMqConnection _connection;

//        private readonly SemaphoreSlim _lock = new(1, 1);

//        private IChannel? _channel;

//        public RabbitMqPublisherChannel(RabbitMqConnection connection)
//        {
//            _connection = connection;
//        }

//        public async Task<IChannel> GetChannelAsync()
//        {
//            if (_channel is { IsOpen: true })
//            {
//                return _channel;
//            }

//            await _lock.WaitAsync();

//            try
//            {
//                if (_channel is { IsOpen: true })
//                {
//                    return _channel;
//                }

//                if (_channel != null)
//                {
//                    await _channel.DisposeAsync();
//                }

//                _channel = await _connection.Connection.CreateChannelAsync();

//                return _channel;
//            }
//            finally
//            {
//                _lock.Release();
//            }
//        }

//        public async ValueTask DisposeAsync()
//        {
//            if (_channel != null)
//            {
//                await _channel.DisposeAsync();
//            }

//            _lock.Dispose();
//        }
//    }
//}
//```