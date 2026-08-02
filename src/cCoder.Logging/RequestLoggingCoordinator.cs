// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Diagnostics;
using System.Security.Claims;
using cCoder.Logging.Brokers;
using cCoder.Logging.Exposures;
using cCoder.Logging.Models;
using Microsoft.AspNetCore.Http.Extensions;

namespace cCoder.Logging;

[System.CodeDom.Compiler.GeneratedCode("cCoder.Logging", "1.0")]
internal sealed class RequestLoggingCoordinator(
    ILogEntryCaptureQueue queue,
    LoggingConfiguration configuration,
    ILogger<RequestLoggingCoordinator> logger)
        : IRequestLoggingCoordinator
{
    public async Task CaptureRequestAsync(HttpContext context, RequestDelegate next)
    {
        if (!configuration.RequestLoggingEnabled || ShouldIgnore(path: context.Request.Path))
        {
            await next(context: context);
            return;
        }

        string domain = context.Request.Host.Host;
        string remoteAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        string userId = context.User?.FindFirstValue(claimType: ClaimTypes.NameIdentifier)
            ?? context.User?.Identity?.Name
            ?? "Guest";
        string method = context.Request.Method;
        string url = context.Request.GetDisplayUrl();
        string correlationId = context.TraceIdentifier;
        long startedTimestamp = Stopwatch.GetTimestamp();

        await next(context: context);

        TimeSpan duration = Stopwatch.GetElapsedTime(startingTimestamp: startedTimestamp);

        LogEntryCaptureRequest request = new()
        {
            Level = LogLevel.Information,
            CategoryName = RequestLoggingMiddleware.CategoryName,
            Message = $"{remoteAddress} as {userId}: {method} - {url} "
                + $"({context.Response.StatusCode}) in {duration.TotalMilliseconds:F1}ms "
                + $"[{correlationId}]",
            RequestDomain = domain,
            Persist = true
        };

        if (!queue.TryEnqueue(request: request))
        {
            logger.LogWarning(message: "Request logging queue is full; the newest request log was dropped.");
        }
    }

    private bool ShouldIgnore(PathString path)
    {
        string loggingRoot = configuration.RootPath?.Trim(trimChar: '/') ?? "Api/Logging";

        return path.StartsWithSegments(
                other: "/Api/Hubs",
                comparisonType: StringComparison.OrdinalIgnoreCase)
            || path.StartsWithSegments(
                other: $"/{loggingRoot}",
                comparisonType: StringComparison.OrdinalIgnoreCase);
    }
}