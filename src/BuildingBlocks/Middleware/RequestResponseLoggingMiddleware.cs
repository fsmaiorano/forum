using System.Diagnostics;
using System.Text;
using BuildingBlocks.Logging;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Middleware;

public class RequestResponseLoggingMiddleware(RequestDelegate next, IAppLogger<RequestResponseLoggingMiddleware> logger)
{
    private const LogType LogType = Logging.LogType.Application;

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? "N/A";

        // Log Request
        await LogRequest(context, correlationId);

        // Capture the original response body stream
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            // Execute the next middleware
            await next(context);

            // Log Response
            stopwatch.Stop();
            await LogResponse(context, correlationId, stopwatch.ElapsedMilliseconds);
        }
        finally
        {
            // Copy the response body back to the original stream
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

    private async Task LogRequest(HttpContext context, string correlationId)
    {
        context.Request.EnableBuffering();

        var requestBody = await ReadRequestBody(context.Request);

        var requestLog = new StringBuilder();
        requestLog.AppendLine(
            $"[REQUEST] {context.Request.Method} {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");
        requestLog.AppendLine($"CorrelationId: {correlationId}");
        requestLog.AppendLine($"Headers: {FormatHeaders(context.Request.Headers)}");

        if (!string.IsNullOrEmpty(requestBody))
        {
            requestLog.AppendLine($"Body: {requestBody}");
        }

        logger.LogInformation(LogType, requestLog.ToString());
    }

    private async Task LogResponse(HttpContext context, string correlationId, long elapsedMilliseconds)
    {
        var responseBody = await ReadResponseBody(context.Response);

        var responseLog = new StringBuilder();
        responseLog.AppendLine($"[RESPONSE] {context.Request.Method} {context.Request.Path}");
        responseLog.AppendLine($"CorrelationId: {correlationId}");
        responseLog.AppendLine($"Status Code: {context.Response.StatusCode}");
        responseLog.AppendLine($"Elapsed Time: {elapsedMilliseconds}ms");
        responseLog.AppendLine($"Headers: {FormatHeaders(context.Response.Headers)}");

        if (!string.IsNullOrEmpty(responseBody))
        {
            responseLog.AppendLine($"Body: {responseBody}");
        }

        logger.LogInformation(LogType, responseLog.ToString());
    }

    private static async Task<string> ReadRequestBody(HttpRequest request)
    {
        request.Body.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();

        request.Body.Seek(0, SeekOrigin.Begin);

        return body;
    }

    private static async Task<string> ReadResponseBody(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(response.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();

        response.Body.Seek(0, SeekOrigin.Begin);

        return body;
    }

    private static string FormatHeaders(IHeaderDictionary headers)
    {
        var headersList = headers
            .Where(h => !IsSensitiveHeader(h.Key))
            .Select(h => $"{h.Key}: {string.Join(", ", h.Value!)}");

        return string.Join("; ", headersList);
    }

    private static bool IsSensitiveHeader(string headerName)
    {
        var sensitiveHeaders = new[] { "Authorization", "Cookie", "Set-Cookie", "X-API-Key" };
        return sensitiveHeaders.Any(h => h.Equals(headerName, StringComparison.OrdinalIgnoreCase));
    }
}