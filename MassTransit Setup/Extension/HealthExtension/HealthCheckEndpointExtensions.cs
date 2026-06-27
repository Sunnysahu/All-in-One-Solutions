using MassTransit_Setup.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;

namespace MassTransit_Setup.Extension.HealthExtension
{
    public static class HealthCheckEndpointExtensions
    {
        public static IApplicationBuilder MapCustomHealthChecks(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json"; 
                  var res = new
                    {
                        Status = report.Status.ToString(),
                        TotalDuration = report.TotalDuration,
                        status = report.Status,
                        Checks = report.Entries.Select(entry => new
                        {
                            Name = entry.Key,
                            Uptime = DateTime.UtcNow - AppInfo.StartedAt,
                            Status = entry.Value.Status.ToString(),
                            Duration = entry.Value.Duration,
                            Description = entry.Value.Description
                        })
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(res));
                }
            });

            return app;
        }
    }
}
