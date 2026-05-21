using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web_Hook.Models;
using Web_Hook.Services.Interfaces;

namespace Web_Hook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly ISignatureService _signatureService;
        private readonly IWebhookService _webhookService;
        private readonly IWebhookQueue _queue;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(ISignatureService signatureService, IWebhookService webhookService, IWebhookQueue queue, ILogger<WebhookController> logger)
        {
            _signatureService = signatureService;
            _webhookService = webhookService;
            _queue = queue;
            _logger = logger;
        }

        [HttpPost("razorpay")]
        public async Task<IActionResult> ReceiveWebhook()
        {
            using var reader = new StreamReader(Request.Body);

            var payload = await reader.ReadToEndAsync();

            var signature = Request.Headers["X-Razorpay-Signature"].ToString();

            if (!_signatureService.IsValid(payload, signature))
            {
                _logger.LogWarning("Invalid Webhook Signature");
                return Unauthorized(new
                {
                    Message = "Invalid Signature",
                    StatusCode = StatusCodes.Status401Unauthorized
                });
            }

            var eventId = Request.Headers["X-Webhook-Id"].ToString();

            Console.WriteLine(eventId);
            var isDuplicate = await _webhookService.IsDuplicateAsync(eventId);

            if (isDuplicate)
            {
                _logger.LogWarning("Duplicate Webhook Received. EventId: {EventId}", eventId);

                return Ok(new
                {
                    Message = "Duplicate Webhook",
                    StatusCode = StatusCodes.Status200OK
                });
            }

            var webhookEvent = new WebhookEvent
            {
                EventId = eventId,
                EventType = "payment.captured", // You can extract this from payload if needed
                Payload = payload,
                Signature = signature,
                CreatedAt = DateTime.UtcNow
            };

            await _webhookService.SaveWebhookEventAsync(webhookEvent);
            await _queue.QueueAsync(webhookEvent);

            _logger.LogInformation("Webhook Received and Queued. EventId: {EventId}", eventId);

            return Ok(new
            {
                Message = "Webhook Accepted",
                StatusCode = StatusCodes.Status200OK
            });
        }
    }
}
// ab740b0f98d602192bae42f96aa6fe6b502a08c8068d0f962bb09ffe584c527b

// {
//     "event": "payment.captured",
//     "account_id": "acc_001",
//     "payload": {
//         "payment": {
//             "entity": {
//                 "id": "pay_001",
//                 "order_id": "order_001",
//                 "amount": 5000,
//                 "currency": "INR",
//                 "status": "captured"
//             }
//         }
//     }
// }

// 1d32921b148a19263d58668b3ee4a120b8bf1ed3ae10b612c76e46ac3c444cd5

//{
//    "event": "payment.captured",
//    "account_id": "acc_002",
//    "payload": {
//        "payment": {
//            "entity": {
//                "id": "pay_002",
//                "order_id": "order_002",
//                "amount": 6000,
//                "currency": "INR",
//                "status": "captured"
//            }
//        }
//    }
//}