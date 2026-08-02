// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Logging;

internal interface IRequestLoggingCoordinator
{
    Task CaptureRequestAsync(HttpContext context, RequestDelegate next);
}