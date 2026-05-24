using MassTransit;
using MassTransit_Setup.Models;

namespace MassTransit_Setup
{
    public class MessagePublisher(IBus bus, ILogger<OrderCreatedEvent> logger) : BackgroundService
    {

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await bus.Publish(new CurrentTime
                {
                    Value = $"The Current time is {DateTime.Now}"
                }, stoppingToken);

                logger.LogWarning("Message Published");

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}