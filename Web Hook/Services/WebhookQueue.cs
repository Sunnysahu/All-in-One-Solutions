using System.Threading.Channels;
using Web_Hook.Models;
using Web_Hook.Services.Interfaces;

namespace Web_Hook.Services
{
    public class WebhookQueue : IWebhookQueue
    {
        private readonly Channel<WebhookEvent> _channel;

        public WebhookQueue()
        {
            var options = new BoundedChannelOptions(100)
            {
                FullMode = BoundedChannelFullMode.Wait
            };

            _channel = Channel.CreateBounded<WebhookEvent>(options);
        }
        public async ValueTask QueueAsync(WebhookEvent webhookEvent)
        {
            await _channel.Writer.WriteAsync(webhookEvent);
        }

        public async ValueTask<WebhookEvent> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _channel.Reader.ReadAsync(cancellationToken);
        }

    }
}
