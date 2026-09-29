// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace cCoder.Logging;

internal interface IRequestLoggingCoordinator
{
    Task CaptureRequestAsync(HttpContext context, RequestDelegate next);
}