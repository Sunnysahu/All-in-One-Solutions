using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EasyNetQ__Rabbit_MQ__Subscriber
{
    public sealed class OrderSubscriberWorker(IBus bus, IServiceProvider serviceProvider, ILogger<OrderSubscriberWorker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Subscriber started. Registering consumers via AutoSubscriber...");

            var autoSubscriber = new AutoSubscriber(bus, serviceProvider, "order-demo");

            await autoSubscriber.SubscribeAsync(typeof(OrderSubscriberWorker).Assembly.GetTypes(),stoppingToken);

            logger.LogInformation("AutoSubscriber ready. Waiting for messages...");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
