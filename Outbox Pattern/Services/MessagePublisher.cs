using Microsoft.Extensions.Options;
using Outbox_Pattern.Messaging;
using Outbox_Pattern.Services.Interface;
using RabbitMQ.Client;
using System.Text;

namespace Outbox_Pattern.Services
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly IRabbitMqConnectionManager _connectionManager;
        private readonly RabbitMqSettings _settings;

        public MessagePublisher(IRabbitMqConnectionManager connectionManager, IOptions<RabbitMqSettings> options)
        {
            _connectionManager = connectionManager;
            _settings = options.Value;
        }

        public Task PublishAsync(string eventType, string payload)
        {
            var connection = _connectionManager.GetConnection();

            using var channel = connection.CreateModel();

            // Exchange
            channel.ExchangeDeclare(
                exchange: _settings.Exchange,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false
            );

            // Queue
            channel.QueueDeclare(
                queue: _settings.Queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // Binding
            channel.QueueBind(
                queue: _settings.Queue, 
                exchange: _settings.Exchange, 
                routingKey: _settings.RoutingKey
            );

            var body = Encoding.UTF8.GetBytes(payload);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: _settings.Exchange,
                routingKey: _settings.RoutingKey,
                basicProperties: properties,
                body: body
            );

            Console.WriteLine("Published to RabbitMQ");

            return Task.CompletedTask;
        }
    }
}
