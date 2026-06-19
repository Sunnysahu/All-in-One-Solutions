using RabbitMQ.Client;

namespace Outbox_Pattern.Messaging
{
    public interface IRabbitMqConnectionManager
    {
        IConnection GetConnection();
    }
}
