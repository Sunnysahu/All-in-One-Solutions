using GlobalAnnotationUtils.Repositories;
using GlobalAnnotationUtils.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.Buffers.Text;
using System.Threading.RateLimiting;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddRequestTimeouts(options =>
//{
//    // Timeout configuration using the default policy for all endpoints
//    options.DefaultPolicy = new RequestTimeoutPolicy
//    {
//        Timeout = TimeSpan.FromSeconds(2),
//        TimeoutStatusCode = StatusCodes.Status408RequestTimeout
//    };

//    // Timeout configuration using a named policy for specific endpoints
//    options.AddPolicy("ShortTimeout", new RequestTimeoutPolicy
//    {
//        Timeout = TimeSpan.FromSeconds(5),
//        TimeoutStatusCode = StatusCodes.Status408RequestTimeout
//    });
//});

// Minimal configuration for Timeouts without using policies
builder.Services.AddRequestTimeouts();

// Fixed Window Limiter

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(
        "fixed",
        config =>
        {
            config.PermitLimit = 10;
            config.Window = TimeSpan.FromMinutes(1);
            config.QueueLimit = 0;
        });
});

// Sliding Window Limiter (Not Notworking as sliding window)

builder.Services.AddRateLimiter(options =>
{
    options.AddSlidingWindowLimiter(
        "sliding",
        config =>
        {
            config.PermitLimit = 5;
            config.Window = TimeSpan.FromMinutes(1);
            config.SegmentsPerWindow = 6;
            config.QueueLimit = 2; // Keep in queue for processing & process after segment overs
        });
});

// Token Bucket Limiter

builder.Services.AddRateLimiter(options =>
{
    options.AddTokenBucketLimiter(
        "token",
        config =>
        {
            config.TokenLimit = 10;
            config.TokensPerPeriod = 2;
            config.ReplenishmentPeriod = TimeSpan.FromSeconds(5);
            config.QueueLimit = 0;
            config.AutoReplenishment = true;
        });
});

// User - Based Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("user-policy", context =>
    {
        //var userId = context.User.Identity?.Name ?? "anonymous";
        var userId = context.Request.Headers["X-User"];

        return RateLimitPartition.GetFixedWindowLimiter(
            userId,
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1)
            });
    });
});

// IP-Based Rate Limiting

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("ip-policy", context =>
    {
        var ip =
            context.Connection.RemoteIpAddress?.ToString() ??
            context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? 
            "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            ip,
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromSeconds(10),
                AutoReplenishment = true,
                QueueLimit = 1,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            });
    });

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            Status = StatusCodes.Status429TooManyRequests,
            Title = "Too Many Requests",
            Detail = "Rate limit exceeded."
        },
        cancellationToken);
    };
});

builder.Services.AddRateLimiter(options =>
{

    int serverCode = StatusCodes.Status503ServiceUnavailable;
    options.AddConcurrencyLimiter("concurrent", config =>
    {
        config.PermitLimit = 1;
        config.QueueLimit = 1;
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.RejectionStatusCode = serverCode;

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = serverCode;

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            Status = serverCode,
            Title = "Too Many Requests",
            Detail = "Server is busy. Please try again later."
        }, cancellationToken);
    };
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   
    app.UseSwaggerUI();
}

app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseRequestTimeouts();

app.UseAuthorization();

app.MapControllers();

app.Run();
