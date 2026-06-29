using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OutBox_Pattern_with_All.Data;

namespace OutBox_Pattern_with_All.HealthChecks
{
    public class SqlHealthCheck : IHealthCheck
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        public SqlHealthCheck(IDbContextFactory<AppDbContext> dbFactory) => _dbFactory = dbFactory;


        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

                bool canConnect = await db.Database.CanConnectAsync(cancellationToken);

                if (!canConnect) return HealthCheckResult.Unhealthy("SQL Server connection failed.");

                return HealthCheckResult.Healthy("SQL Server is healthy.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("SQL Server is unhealthy.", ex);
            }
        }
    }
}
