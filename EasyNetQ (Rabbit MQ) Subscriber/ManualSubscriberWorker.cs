
using EasyNetQ;
using Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EasyNetQ__Rabbit_MQ__Subscriber
{
    public sealed class ManualSubscriberWorker(IBus bus, ILogger<ManualSubscriberWorker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Subscriber started. Waiting for orders...");

            await bus.PubSub.SubscribeAsync<OrderPlacedMessage>(
                subscriptionId: "order_demo", 
                onMessage : HandleOrderAsync, 
                cancellationToken: stoppingToken
            );

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private Task HandleOrderAsync(OrderPlacedMessage message)
        {
            logger.LogInformation($"Received Order {message.OrderId} for {message.CustomerName} - ${message.Amount}");
            logger.LogInformation("Received Order {OrderId} for {Customer} - ${Amount}", message.OrderId, message.CustomerName, message.Amount);

            return Task.CompletedTask;
        }
    }
}
