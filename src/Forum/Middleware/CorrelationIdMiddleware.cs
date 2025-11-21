using Microsoft.Extensions.Primitives;

namespace Forum.Middleware;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrAddCorrelationId(context);
        AddCorrelationIdToResponse(context, correlationId);
        await next(context);
    }

    private static StringValues GetOrAddCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
            return correlationId;

        correlationId = Guid.NewGuid().ToString();
        context.Request.Headers.Append(CorrelationIdHeader, correlationId);
        return correlationId;
    }

    private static void AddCorrelationIdToResponse(HttpContext context, StringValues correlationId)
    {
        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.ContainsKey(CorrelationIdHeader))
                context.Response.Headers.Append(CorrelationIdHeader, correlationId);

            return Task.CompletedTask;
        });
    }
}