using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;
using Web_Hook.Services.Interfaces;

namespace Web_Hook.Services
{
    public class SignatureService : ISignatureService
    {
        private readonly IConfiguration _configuration;

        public SignatureService(IConfiguration configuration) => _configuration = configuration;

        public bool IsValid(string payload, string incomingSignature)
        {
            var serect = _configuration["Webhook:Secret"];
            var keyBytes = Encoding.UTF8.GetBytes(serect!);
            var payloadBytes = Encoding.UTF8.GetBytes(payload);

            using var hmac = new HMACSHA256(keyBytes);

            var hash = hmac.ComputeHash(payloadBytes);

            var generatedSignature = Convert.ToHexString(hash).ToLower();

            return generatedSignature == incomingSignature.ToLower();
        }
    }
}
