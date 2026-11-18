// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using cCoder.Logging.Brokers.Loggings;

namespace cCoder.Logging.Exposures;

[System.CodeDom.Compiler.GeneratedCode("cCoder.Logging", "1.0")]
internal sealed class ControllerFailureLoggingFilter(
    ILoggingBroker loggingBroker)
        : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(
        ResultExecutingContext context,
        ResultExecutionDelegate next)
    {
        ArgumentNullException.ThrowIfNull(argument: context);
        ArgumentNullException.ThrowIfNull(argument: next);

        ResultExecutedContext executedContext = await next();
        int statusCode = executedContext.HttpContext.Response.StatusCode;

        if (statusCode < StatusCodes.Status400BadRequest)
        {
            return;
        }

        string reason = SelectReason(result: context.Result);

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            loggingBroker.LogError(
                message: $"Controller request returned {statusCode}. Reason: {reason}");
        }
        else
        {
            loggingBroker.LogWarning(
                message: $"Controller request returned {statusCode}. Reason: {reason}");
        }
    }

    private static string SelectReason(IActionResult result) =>
        result is ObjectResult objectResult
            ? objectResult.Value?.ToString() ?? "No reason was supplied."
            : "No reason was supplied.";
}