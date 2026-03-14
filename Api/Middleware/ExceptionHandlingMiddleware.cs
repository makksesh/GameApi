using System.Text.Json;
using Application.Common.Exceptions;

namespace Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException      e => (StatusCodes.Status404NotFound,       e.Message),
            ConflictException      e => (StatusCodes.Status409Conflict,       e.Message),
            ForbiddenException     e => (StatusCodes.Status403Forbidden,      e.Message),
            BusinessRuleException  e => (StatusCodes.Status422UnprocessableEntity, e.Message),
            _                        => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = statusCode;

        var response = JsonSerializer.Serialize(new
        {
            statusCode,
            message
        });

        await context.Response.WriteAsync(response);
    }
}