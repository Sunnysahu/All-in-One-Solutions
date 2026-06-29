using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using OutBox_Pattern_with_All.Data;
using OutBox_Pattern_with_All.HealthChecks;
using OutBox_Pattern_with_All.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddDbContext<AppDbContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
//});

builder.Services.AddDbContextFactory<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSingleton<RabbitMqConnection>();

//builder.Services.AddSingleton<RabbitMqInitializer>();

builder.Services.AddSingleton<RabbitMqPublisher>();

builder.Services.AddSingleton<MessageProcessor>();

builder.Services.AddSingleton<OutboxMessageLeaseService>();

builder.Services.AddSingleton<RabbitMqTopology>();

builder.Services.AddHostedService<OutboxProcessorService>();

builder.Services.AddHostedService<RabbitMqConsumer>();

builder.Services.AddHostedService<CleanupService>();

builder.Services.AddHttpClient<RabbitMqManagementService>();

builder.Services.AddHealthChecks()
    .AddCheck<SqlHealthCheck>("SQL Server")
    .AddCheck<RabbitMqHealthCheck>("RabbitMQ");

var app = builder.Build();

app.Services.GetRequiredService<RabbitMqTopology>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var result = new
        {
            Status = report.Status.ToString(),

            Checks = report.Entries.Select(entry => new
            {
                Name = entry.Key,
                Status = entry.Value.Status.ToString(),
                Description = entry.Value.Description
            })
        };

        await context.Response.WriteAsync(
        JsonSerializer.Serialize(result,
        new JsonSerializerOptions
        {
            WriteIndented = true
        }));
    }
});
app.MapControllers();

app.Run();
