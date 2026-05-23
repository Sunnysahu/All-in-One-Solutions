using System.Text.Json.Serialization;

namespace Web_Hook.DTOs
{
    public class RazorpayWebhookDto
    {
        [JsonPropertyName("event")]
        public string Event { get; set; }
        [JsonPropertyName("account_id")]
        public string AccountId { get; set; }
        [JsonPropertyName("payload")]
        public PaymentPayload Payload { get; set; }

    }

    public class PaymentPayload
    {
        [JsonPropertyName("payment")]
        public PaymentEntity Payment { get; set; }
    }

    public class PaymentEntity
    {
        [JsonPropertyName("entity")]
        public PaymentCapturedDto Entity { get; set; }
    }
}
