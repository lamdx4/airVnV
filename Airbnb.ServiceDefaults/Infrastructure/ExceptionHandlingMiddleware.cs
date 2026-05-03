using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Airbnb.ServiceDefaults.Infrastructure;

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
            logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, errorCode, message) = exception switch
        {
            NotFoundException ex => (HttpStatusCode.NotFound, "NOT_FOUND", ex.Message),
            BusinessException ex => (HttpStatusCode.BadRequest, ex.ErrorCode, ex.Message),
            InvalidOperationException ex => (HttpStatusCode.BadRequest, "INVALID_OPERATION", ex.Message),
            ArgumentException ex => (HttpStatusCode.BadRequest, "BAD_REQUEST", ex.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "UNAUTHORIZED", "Access denied."),
            _ => (HttpStatusCode.InternalServerError, "INTERNAL_SERVER_ERROR", "An unexpected error occurred.")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.FailureResult(errorCode, message);
        
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
