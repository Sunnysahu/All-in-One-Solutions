using OutBox_Pattern_with_All.Constants;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace OutBox_Pattern_with_All.Services
{
    public class RabbitMqConsumer : BackgroundService
    {
        private readonly RabbitMqConnection _connection;
        private readonly MessageProcessor _messageProcessor;

        private IChannel? _channel;

        public RabbitMqConsumer(RabbitMqConnection connection, MessageProcessor messageProcessor)
        {
            _connection = connection;
            _messageProcessor = messageProcessor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel = await _connection.Connection.CreateChannelAsync();

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += OnMessageReceived;

            await _channel.BasicConsumeAsync(
                queue: RabbitMqConstants.Queue,
                autoAck: false,
                consumer: consumer
            );

            Console.WriteLine("RabbitMQ Consumer Started...");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task OnMessageReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            try
            {
                string payload = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

                await _messageProcessor.ProcessAsync(payload);

                await _channel!.BasicAckAsync(eventArgs.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                await _channel!.BasicNackAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: true
                );
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null)
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
            }

            await base.StopAsync(cancellationToken);
        }
    }
}
