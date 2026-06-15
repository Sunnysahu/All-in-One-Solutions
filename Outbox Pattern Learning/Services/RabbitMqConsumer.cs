using Microsoft.Extensions.Options;
using Outbox_Pattern_Learning.Configuration;
using Outbox_Pattern_Learning.Repositories.Interfaces;
using Outbox_Pattern_Learning.Services.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Outbox_Pattern_Learning.Services
{
    // This class is the transport adapter for inbound RabbitMQ messages.
    public sealed class RabbitMqConsumer : IRabbitMqConsumer
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly RabbitMqOptions _options;
        private readonly ILogger<RabbitMqConsumer> _logger;

        private readonly ConnectionFactory _factory;

        private IConnection? _connection;
        private IChannel? _channel;

        public bool IsRunning { get; private set; }

        public bool IsConnected =>
            _connection is not null &&
            _connection.IsOpen &&
            _channel is not null;

        public RabbitMqConsumer(
            IServiceScopeFactory scopeFactory,
            IOptions<RabbitMqOptions> options,
            ILogger<RabbitMqConsumer> logger
        )
        {
            _scopeFactory = scopeFactory;
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

        public async Task StartAsync(
        CancellationToken cancellationToken = default)
        {
            if (IsRunning) return;

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

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += OnMessageReceivedAsync;

            await _channel.BasicConsumeAsync(
                queue: _options.QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: cancellationToken
            );

            IsRunning = true;

            _logger.LogInformation("RabbitMQ consumer started.");
        }

        private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs args)
        {
            using var scope = _scopeFactory.CreateScope();

            var processedRepository =
                scope.ServiceProvider.GetRequiredService<IProcessedMessageRepository>();

            try
            {
                var json = Encoding.UTF8.GetString(args.Body.ToArray());

                using var document = JsonDocument.Parse(json);

                var root = document.RootElement;

                var messageId =
                    Guid.Parse(root.GetProperty("messageId").GetString()!);

                var alreadyProcessed = await processedRepository.ExistsAsync(messageId);

                if (alreadyProcessed)
                {
                    _logger.LogInformation(
                        "Duplicate message {MessageId} ignored.",
                        messageId
                    );

                    await _channel!.BasicAckAsync(
                        args.DeliveryTag,
                        false
                    );

                    return;
                }

                // --------------------------------------------------
                // Business processing should happen here.
                //
                // In a production implementation this should invoke:
                //
                // IOrderEventHandler
                //
                // rather than contain business logic directly.
                // --------------------------------------------------

                await processedRepository.CreateAsync(
                    new Models.ProcessedMessage
                    {
                        Id = Guid.NewGuid(),
                        MessageId = messageId,
                        ProcessedAt = DateTime.UtcNow
                    });

                await processedRepository.SaveChangesAsync();

                await _channel!.BasicAckAsync(
                    args.DeliveryTag,
                    false);

                _logger.LogInformation(
                    "Processed message {MessageId}.",
                    messageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Message processing failed."
                );

                await _channel!.BasicNackAsync(
                    args.DeliveryTag,
                    false,
                    true
                );
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (!IsRunning)
            {
                return;
            }

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

            IsRunning = false;

            _logger.LogInformation(
                "RabbitMQ consumer stopped.");
        }

        public async ValueTask DisposeAsync()
        {
            await StopAsync();
        }
    }
}
