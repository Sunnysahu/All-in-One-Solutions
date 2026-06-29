using OutBox_Pattern_with_All.Models;
using System.Net.Http.Headers;
using System.Text;

namespace OutBox_Pattern_with_All.Services
{
    public class RabbitMqManagementService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public RabbitMqManagementService(
            HttpClient client,
            IConfiguration configuration
        )
        {
            _client = client;
            _configuration = configuration;

            string username = configuration["RabbitMqManagement:Username"]!;
            string password = configuration["RabbitMqManagement:Password"]!;

            string token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);

            _client.BaseAddress = new Uri(configuration["RabbitMqManagement:BaseUrl"]!);
        }

        public async Task<List<QueueInfo>?> GetQueuesAsync()
        {
            var vhost = _configuration["RabbitMqManagement:VHost"]!;

            var response = await _client.GetAsync($"queues/{vhost}");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<QueueInfo>>();
        }
    }
}
