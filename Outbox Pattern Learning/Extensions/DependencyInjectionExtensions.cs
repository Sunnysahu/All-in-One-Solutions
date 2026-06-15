using Microsoft.EntityFrameworkCore;
using Outbox_Pattern_Learning.BackgroundServices;
using Outbox_Pattern_Learning.Configuration;
using Outbox_Pattern_Learning.Data;
using Outbox_Pattern_Learning.Repositories;
using Outbox_Pattern_Learning.Repositories.Interfaces;
using Outbox_Pattern_Learning.Services;
using Outbox_Pattern_Learning.Services.Interfaces;

namespace Outbox_Pattern_Learning.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // ----------------------------------------------------
            // Options
            // ----------------------------------------------------

            services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));

            services.Configure<OutboxOptions>(configuration.GetSection("Outbox"));

            // ----------------------------------------------------
            // DbContext
            // ----------------------------------------------------

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

            // ----------------------------------------------------
            // Repositories
            // ----------------------------------------------------

            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddScoped<IOutboxRepository, OutboxRepository>();

            services.AddScoped<IProcessedMessageRepository, ProcessedMessageRepository>();

            // ----------------------------------------------------
            // Business Services
            // ----------------------------------------------------

            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<IOutboxService, OutboxService>();

            // ----------------------------------------------------
            // RabbitMQ Services
            // ----------------------------------------------------

            services.AddSingleton<IRabbitMqPublisher,RabbitMqPublisher>();

            services.AddSingleton<IRabbitMqConsumer,RabbitMqConsumer>();

            // ----------------------------------------------------
            // Background Workers
            // ----------------------------------------------------

            services.AddHostedService<OutboxPublisherBackgroundService>();

            // ----------------------------------------------------
            // Controllers
            // ----------------------------------------------------

            services.AddControllers();

            // ----------------------------------------------------
            // Swagger
            // ----------------------------------------------------

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();

            return services;
        }
    }
}
