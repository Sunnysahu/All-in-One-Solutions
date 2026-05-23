using Hangfire;
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
            
            BackgroundJob.Enqueue<IWebhookService>(x => x.ProcessWebhookAsync(webhookEvent)); // Comment this and Uncomment the lower lines to use BackGourn Service

            //BackgroundJob.Enqueue(() => Console.WriteLine("Webhook Processing"));
            //await _channel.Writer.WriteAsync(webhookEvent);
            //await Task.Delay(1000);
        }

        public async ValueTask<WebhookEvent> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _channel.Reader.ReadAsync(cancellationToken);
        }

    }
}
