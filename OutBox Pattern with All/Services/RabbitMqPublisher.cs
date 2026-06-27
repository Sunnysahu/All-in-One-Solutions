using OutBox_Pattern_with_All.Constants;
using RabbitMQ.Client;
using System.Text;

namespace OutBox_Pattern_with_All.Services
{
    public class RabbitMqPublisher
    {
        private readonly RabbitMqInitializer _initializer;


        public RabbitMqPublisher(RabbitMqInitializer initializer)
        {
            _initializer = initializer;
        }


        public async Task PublishAsync(string payload)
        {
            var body = Encoding.UTF8.GetBytes(payload);
            var props = new BasicProperties
            {
                Persistent = true
            };

            await _initializer.Channel.BasicPublishAsync(
                exchange: RabbitMqConstants.Exchange,
                routingKey: RabbitMqConstants.RoutingKey,
                mandatory: true,
                basicProperties: props,
                body: body
            );
        }
    }
}
