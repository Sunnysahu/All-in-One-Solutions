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
                ClientProvidedName = "OutboxProcessorService",
                AutomaticRecoveryEnabled = true, // We'll enable automatic network recovery
                TopologyRecoveryEnabled= true, // Recover topology after reconnect
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10) // Retry every 10 seconds

            };

            Connection = factory
                .CreateConnectionAsync()
                .GetAwaiter()
                .GetResult();
        }

        public async ValueTask DisposeAsync()
        {
            Console.WriteLine("Closing RabbitMQ Connection...");

            await Connection.CloseAsync();

            await Connection.DisposeAsync();
        }
    }
}
