using System.Text.Json;
using Forum.BuildingBlocks.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Forum.BuildingBlocks.Exceptions;

public class CustomExceptionHandler(IAppLogger<CustomExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var correlationId = httpContext.Request.Headers["X-Correlation-ID"].FirstOrDefault() ??
                            httpContext.TraceIdentifier;

        LogException(httpContext, exception, correlationId);

        (string Detail, string Title, int StatusCode) details = exception switch
        {
            InternalServerException => (
                exception.Message,
                exception.GetType().Name,
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError
            ),
            NotFoundException => (
                exception.Message,
                exception.GetType().Name,
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound
            ),
            BadRequestException => (
                exception.Message,
                exception.GetType().Name,
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest
            ),
            ValidationException => (
                exception.Message,
                exception.GetType().Name,
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest
            ),
            _ => (
                exception.Message,
                exception.GetType().Name,
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError
            )
        };

        var problemDetails = new ProblemDetails
        {
            Title = details.Title,
            Status = details.StatusCode,
            Detail = details.Detail,
        };

        problemDetails.Extensions.Add("traceId", httpContext.TraceIdentifier);
        problemDetails.Extensions.Add("correlationId", correlationId);

        if (exception is ValidationException validationException)
            problemDetails.Extensions.Add("ValidationErrors", validationException.Errors);

        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(problemDetails),
            cancellationToken
        );

        return true;
    }

    private void LogException(HttpContext httpContext, Exception exception, string correlationId)
    {
        var logMessage = new StringBuilder();
        logMessage.AppendLine($"[EXCEPTION CAUGHT]");
        logMessage.AppendLine($"CorrelationId: {correlationId}");
        logMessage.AppendLine($"TraceId: {httpContext.TraceIdentifier}");
        logMessage.AppendLine($"Timestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} UTC");
        logMessage.AppendLine($"Exception Type: {exception.GetType().FullName}");
        logMessage.AppendLine($"Message: {exception.Message}");
        logMessage.AppendLine(
            $"Request: {httpContext.Request.Method} {httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}{httpContext.Request.QueryString}");
        logMessage.AppendLine($"User: {httpContext.User.Identity?.Name ?? "Anonymous"}");
        logMessage.AppendLine($"IP Address: {httpContext.Connection.RemoteIpAddress}");

        switch (exception)
        {
            case BadRequestException badRequestException when !string.IsNullOrEmpty(badRequestException.Details):
                logMessage.AppendLine($"Details: {badRequestException.Details}");
                break;
            case InternalServerException internalServerException
                when !string.IsNullOrEmpty(internalServerException.Details):
                logMessage.AppendLine($"Details: {internalServerException.Details}");
                break;
            case ValidationException validationException when validationException.Errors.Any():
                logMessage.AppendLine($"Validation Errors: {string.Join(", ", validationException.Errors)}");
                break;
        }

        if (exception.InnerException != null)
        {
            logMessage.AppendLine($"Inner Exception: {exception.InnerException.GetType().FullName}");
            logMessage.AppendLine($"Inner Exception Message: {exception.InnerException.Message}");
        }

        logMessage.AppendLine($"Stack Trace:");
        logMessage.AppendLine(exception.StackTrace ?? "No stack trace available");

        logger.LogError(LogType.Exception, exception, logMessage.ToString());
    }
}