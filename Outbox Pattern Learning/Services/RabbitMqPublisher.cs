using Microsoft.Extensions.Options;
using Outbox_Pattern_Learning.Configuration;
using Outbox_Pattern_Learning.Models;
using Outbox_Pattern_Learning.Services.Interfaces;
using RabbitMQ.Client;
using System.Text;

namespace Outbox_Pattern_Learning.Services
{
    public sealed class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly RabbitMqOptions _options;
        private readonly ILogger<RabbitMqPublisher> _logger;

        private readonly ConnectionFactory _factory;

        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMqPublisher(IOptions<RabbitMqOptions> options, ILogger<RabbitMqPublisher> logger)
        {
            _options = options.Value;
            _logger = logger;

            _factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost
            };
        }

        public bool IsConnected => 
            _connection is not null && _connection.IsOpen && _channel is not null;

        public async Task ConnectAsync(
       CancellationToken cancellationToken = default)
        {
            if (IsConnected)
            {
                return;
            }

            _connection = await _factory.CreateConnectionAsync(cancellationToken);

            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await _channel.ExchangeDeclareAsync(
                exchange: _options.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken
            );

            await _channel.QueueDeclareAsync(
                queue: _options.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );

            await _channel.QueueBindAsync(
                queue: _options.QueueName,
                exchange: _options.ExchangeName,
                routingKey: _options.RoutingKey,
                cancellationToken: cancellationToken
            );

            _logger.LogInformation("RabbitMQ connection established.");
        }

        public async Task<bool> PublishAsync(
            OutboxMessage message,
            CancellationToken cancellationToken = default)
        {
            if (!IsConnected)
            {
                await ConnectAsync(cancellationToken);
            }

            var body = Encoding.UTF8.GetBytes(message.Payload);

            var properties = new BasicProperties
            {
                MessageId = message.MessageId.ToString(),
                CorrelationId = message.CorrelationId.ToString(),
                Persistent = true,
                ContentType = "application/json",
                Type = message.EventType
            };

            await _channel!.BasicPublishAsync(
                exchange: _options.ExchangeName,
                routingKey: _options.RoutingKey,
                mandatory: true,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken
            );

            _logger.LogInformation(
                "RabbitMQ published message {MessageId}.", 
                message.MessageId
            );

            // In production this should wait for publisher confirms.
            return true;
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            if (_channel is not null)
            {
                await _channel.CloseAsync(cancellationToken);
                await _channel.DisposeAsync();
                _channel = null;
            }

            if (_connection is not null)
            {
                await _connection.CloseAsync(cancellationToken);
                await _connection.DisposeAsync();
                _connection = null;
            }

            _logger.LogInformation("RabbitMQ connection closed.");
        }

        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync();
        }
    }
}
