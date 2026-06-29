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

                Console.WriteLine("--------------------------------");
                Console.WriteLine("Message Received");
                await _messageProcessor.ProcessAsync(payload);

                Console.WriteLine("Sending ACK");
                await _channel!.BasicAckAsync(eventArgs.DeliveryTag, false);
                Console.WriteLine("--------------------------------");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                int retryCount = GetRetryCount(eventArgs);

                Console.WriteLine($"Retry Count : {retryCount}");

                if (retryCount >= 5)
                {
                    Console.WriteLine("Moving to DLQ");

                    await _channel!.BasicPublishAsync(
                        exchange: RabbitMqConstants.Exchange,
                        routingKey: RabbitMqConstants.DeadLetterRoutingKey,
                        mandatory: true,
                        basicProperties: new BasicProperties
                        {
                            Persistent = true
                        },
                        body: eventArgs.Body
                    );

                    await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);

                    return;
                }

                await _channel!.BasicNackAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: false
                );

                Console.WriteLine("Sent To Retry Queue");
            }
        }

        public static int GetRetryCount(BasicDeliverEventArgs eventArgs)
        {
            if (eventArgs.BasicProperties.Headers == null)
                return 0;

            if (!eventArgs.BasicProperties.Headers.TryGetValue("x-death", out var value))
                return 0;

            if (value is not IList<object> deaths || deaths.Count == 0)
                return 0;

            if (deaths[0] is not Dictionary<string, object> death)
                return 0;

            if (!death.TryGetValue("count", out var count))
                return 0;

            return Convert.ToInt32(count);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Stopping RabbitMQ Consumer...");

            if (_channel is not null) await _channel.CloseAsync();

            await base.StopAsync(cancellationToken);
        }


    }
}
