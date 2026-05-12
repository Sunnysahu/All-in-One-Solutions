using EasyNetQ;
using Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Publisher;

public sealed class OrderPublisherWorker(IBus bus, ILogger<OrderPublisherWorker> logger): BackgroundService
{
    private static readonly string[] CustomerNames = ["Sunny Sahu", "Ariba Siddiqui", "Aditi Vishwakarma", "Roshni Shiekh", "Chicken Butter Masala"];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Publisher started. Sending an order every 3 seconds");

        var random = new Random();
        var orderNumber = 1;

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(3));

        while (await timer.WaitForNextTickAsync())
        {
            var message = new OrderPlacedMessage(
                OrderId: Guid.NewGuid(),
                CustomerName: CustomerNames[random.Next(CustomerNames.Length)],
                //Amount: Math.Round(10m + (decimal) random.NextDouble() * (500m - 10m), 2)
                Amount: Math.Round(random.NextDecimal(10m, 500m), 2)
                );

            await bus.PubSub.PublishAsync(message, stoppingToken);

            logger.LogInformation($"Published Order #{orderNumber++} : {message.OrderId} : {message.CustomerName} : {message.Amount}");
        }
    }
}

// Decimal Extension use Above

public static class RandomExtensions
{
    public static decimal NextDecimal(this Random random, decimal min, decimal max)
    {
        var next = (decimal) random.NextDouble();
        return min + (next * (max - min));
    }
}