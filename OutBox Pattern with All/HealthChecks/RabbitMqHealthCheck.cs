using Microsoft.Extensions.Diagnostics.HealthChecks;
using OutBox_Pattern_with_All.Services;

namespace OutBox_Pattern_with_All.HealthChecks
{
    public class RabbitMqHealthCheck : IHealthCheck
    {
        private readonly RabbitMqConnection _connection;

        public RabbitMqHealthCheck(RabbitMqConnection connection) => _connection = connection;

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                if (_connection.Connection == null) 
                    return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ connection is null."));

                if (!_connection.Connection.IsOpen)
                    return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ connection is closed."));

                return Task.FromResult(HealthCheckResult.Healthy("RabbitMQ connection is healthy."));
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ health check failed.", ex));
            }
        }
    }
}
