using Microsoft.AspNetCore.Mvc;

namespace GlobalExceptionAndLogging.Exceptions;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(
                ex,
                "Product was not found. Path: {Path}",
                context.Request.Path
            );

            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status404NotFound,
                "Resource not found.",
                ex.Message
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception. Path: {Path}",
                context.Request.Path
            );

            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.",
                null
            );
        }
    }

    private static async Task WriteProblemDetailsAsync(HttpContext context, int statusCode, string title, string? detail)
    {
        context.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path,
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}