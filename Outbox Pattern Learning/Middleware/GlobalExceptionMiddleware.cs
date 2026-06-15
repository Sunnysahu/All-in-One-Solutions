using System.Net;
using System.Text.Json;

namespace Outbox_Pattern_Learning.Middleware
{
    public sealed class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "Request was cancelled. TraceId: {TraceId}",
                    context.TraceIdentifier);

                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode =
                        StatusCodes.Status499ClientClosedRequest;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception occurred. TraceId: {TraceId}",
                    context.TraceIdentifier);

                await WriteErrorResponseAsync(
                    context,
                    ex);
            }
        }

        private async Task WriteErrorResponseAsync(HttpContext context, Exception exception)
        {
            if (context.Response.HasStarted) return;

            context.Response.Clear();

            context.Response.ContentType = "application/json";

            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

            var response = new ErrorResponse
            {
                Success = false,
                StatusCode = context.Response.StatusCode,
                Message = "An unexpected error occurred.",
                TraceId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow,

                Details = _environment.IsDevelopment() ? exception.ToString() : null
            };

            var json = JsonSerializer.Serialize(
                response,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                }
            );

            await context.Response.WriteAsync(json);
        }

        private sealed class ErrorResponse
        {
            public bool Success { get; set; }

            public int StatusCode { get; set; }

            public string Message { get; set; } = string.Empty;

            public string TraceId { get; set; } = string.Empty;

            public DateTime Timestamp { get; set; }

            public string? Details { get; set; }
        }
    }
}
