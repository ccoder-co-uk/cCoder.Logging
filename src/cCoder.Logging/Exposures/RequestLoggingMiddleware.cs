// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.Logging.Exposures;

internal sealed class RequestLoggingMiddleware(
    IRequestLoggingCoordinator requestLoggingCoordinator)
        : IMiddleware
{
    internal const string CategoryName = "cCoder.Request";

    Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next) =>
        requestLoggingCoordinator.CaptureRequestAsync(
            context: context,
            next: next);
}