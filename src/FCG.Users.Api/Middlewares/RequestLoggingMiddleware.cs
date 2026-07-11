using FCG.Users.Application.Contracts;
using System.Diagnostics;

namespace FCG.Users.Api.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context, ICorrelationIdAccessor correlationIdAccessor)
    {
        var correlationId = correlationIdAccessor.Get();

        _logger.LogInformation(
            "Request started {Method} {Path} CorrelationId {CorrelationId}",
            context.Request.Method,
            context.Request.Path,
            correlationId);

        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        _logger.LogInformation(
            "Request finished {Method} {Path} StatusCode {StatusCode} DurationMs {DurationMs} CorrelationId {CorrelationId}",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds,
            correlationIdAccessor.Get());
    }
}