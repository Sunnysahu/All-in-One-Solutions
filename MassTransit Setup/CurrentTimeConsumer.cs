using MassTransit;
using MassTransit_Setup.Models;

namespace MassTransit_Setup
{
    public class CurrentTimeConsumer(ILogger<CurrentTimeConsumer> logger) : IConsumer<CurrentTime>
    {
        public Task Consume(ConsumeContext<CurrentTime> context)
        {
            logger.LogInformation($"{nameof(CurrentTimeConsumer)} : {context.Message.Value}");
            return Task.CompletedTask;
        }
    }
    public class CurrentTimeConsumerV2(ILogger<CurrentTimeConsumer> logger) : IConsumer<CurrentTime>
    {
        public Task Consume(ConsumeContext<CurrentTime> context)
        {
            logger.LogInformation($"{nameof(CurrentTimeConsumerV2)} : {context.Message.Value}");
            return Task.CompletedTask;
        }
    }
}
