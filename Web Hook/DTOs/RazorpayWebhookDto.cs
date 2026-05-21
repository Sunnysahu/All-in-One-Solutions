using System.Text.Json.Serialization;

namespace Web_Hook.DTOs
{
    public class RazorpayWebhookDto
    {
        [JsonPropertyName("event")]
        public string Event { get; set; } = default;
        [JsonPropertyName("account_id")]
        public string AccountId { get; set; } = default;
        [JsonPropertyName("payload")]
        public PaymentPayload Payload { get; set; } = default;

    }

    public class PaymentPayload
    {
        [JsonPropertyName("payment")]
        public PaymentEntity Payment { get; set; } = default;
    }

    public class PaymentEntity
    {
        [JsonPropertyName("entity")]
        public PaymentCapturedDto Entity { get; set; } = default;
    }
}
