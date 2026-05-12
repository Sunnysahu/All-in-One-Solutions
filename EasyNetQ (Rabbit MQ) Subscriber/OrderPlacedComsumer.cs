using EasyNetQ.AutoSubscribe;
using Messages;
using Microsoft.Extensions.Logging;

namespace EasyNetQ__Rabbit_MQ__Subscriber
{
    public sealed class OrderPlacedComsumer(ILogger<OrderPlacedComsumer> logger) : IConsumeAsync<OrderPlacedMessage>
    {
        [AutoSubscriberConsumer(SubscriptionId = "order-placed-consumer")]  
        public Task ConsumeAsync(OrderPlacedMessage message, CancellationToken cancellationToken = default)
        {
            logger.LogInformation($"Received order {message.OrderId} for {message.CustomerName} - ${message.Amount}");

            return Task.CompletedTask;
        }
    }
}