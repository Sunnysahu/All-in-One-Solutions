namespace Outbox_Pattern.Services.Interface
{
    public interface IMessagePublisher
    {
        Task PublishAsync(string eventType, string payload);
    }
}
