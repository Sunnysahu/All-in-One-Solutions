namespace Outbox_Pattern_Learning.Configuration
{
    public sealed class RabbitMqOptions
    {
        public string HostName { get; set; } = string.Empty;

        public int Port { get; set; } = 5672;

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string VirtualHost { get; set; } = "/";

        public string ExchangeName { get; set; } = string.Empty;

        public string ExchangeType { get; set; } = "topic";

        public string QueueName { get; set; } = string.Empty;

        public string RoutingKey { get; set; } = string.Empty;
        public bool EnablePublisherConfirms { get; set; } = true;

        public bool AutomaticRecoveryEnabled { get; set; } = true;

        public int NetworkRecoveryIntervalSeconds { get; set; } = 10;

        public int RequestedHeartbeatSeconds { get; set; } = 60;

        public ushort PrefetchCount { get; set; } = 20;

    }
}

/*
How it interacts with other classes

appsettings.json
        │
        ▼
RabbitMqOptions
        │
        ▼
DependencyInjectionExtensions
        │
        ▼
IOptions<RabbitMqOptions>
        │
 ┌──────┴────────┐
 ▼               ▼
RabbitMqPublisher RabbitMqConsumer


--------------------------------------------

Why this file exists

Instead of writing:

var host = configuration["RabbitMq:HostName"];
var port = configuration["RabbitMq:Port"];
var exchange = configuration["RabbitMq:ExchangeName"];
var queue = configuration["RabbitMq:QueueName"];

throughout the project, ASP.NET Core can bind the configuration to a strongly typed class.

This class acts as a single source of truth for all RabbitMQ settings.
 */
