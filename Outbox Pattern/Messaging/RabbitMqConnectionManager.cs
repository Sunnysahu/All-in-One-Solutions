using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Outbox_Pattern.Messaging
{

    public class RabbitMqConnectionManager : IRabbitMqConnectionManager
    {
        private readonly IConnection _connection;

        public RabbitMqConnectionManager(IOptions<RabbitMqSettings> options)
        {
            var setting = options.Value;

            var factory = new ConnectionFactory
            {
                HostName = setting.Host,
                Port = setting.Port,
                UserName = setting.Username,
                Password = setting.Password
            };

            _connection = factory.CreateConnection();
        }

        public IConnection GetConnection()
        {
            return _connection;
        }
    }
}
