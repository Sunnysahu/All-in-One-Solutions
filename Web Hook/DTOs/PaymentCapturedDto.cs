using System.Text.Json.Serialization;

namespace Web_Hook.DTOs
{
    public class PaymentCapturedDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("order_id")]
        public string OrderId { get; set; } = default!;

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = default!;

        [JsonPropertyName("status")]
        public string Status { get; set; } = default!;
    }
}
