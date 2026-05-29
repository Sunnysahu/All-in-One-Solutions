using MassTransit;
using MassTransit_Using_RabbitMQ.Consumers;

namespace MassTransit_Using_RabbitMQ.Configurations
{
    public static class MassTransitConfiguration
    {
        public static IServiceCollection AddMassTransitConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<OrderCreatedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint("order-created-queue", e =>
                    {
                        e.ConfigureConsumer<OrderCreatedConsumer>(context);

                        e.UseMessageRetry(r =>
                        {
                            r.Interval(3, TimeSpan.FromSeconds(5));
                            // r.Incremental(3, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
                        });
                    });
                });
            });
            
            return services;
        }
    }
}
