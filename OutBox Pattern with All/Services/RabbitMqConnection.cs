using RabbitMQ.Client;

namespace OutBox_Pattern_with_All.Services
{
    public sealed class RabbitMqConnection : IAsyncDisposable
    {
        public IConnection Connection { get; }

        public RabbitMqConnection(IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMq:HostName"]!,
                ClientProvidedName = "OutboxProcessorService"
            };

            Connection = factory
                .CreateConnectionAsync()
                .GetAwaiter()
                .GetResult();
        }

        public async ValueTask DisposeAsync()
        {
            await Connection.DisposeAsync();
        }
    }
}
