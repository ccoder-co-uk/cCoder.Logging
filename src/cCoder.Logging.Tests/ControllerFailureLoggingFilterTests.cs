// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;

using cCoder.Logging.Brokers.Loggings;
using cCoder.Logging.Exposures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace cCoder.Logging.Tests;

public sealed partial class ControllerFailureLoggingFilterTests
{
    [Theory]
    [InlineData(StatusCodes.Status400BadRequest, LogLevel.Warning)]
    [InlineData(StatusCodes.Status500InternalServerError, LogLevel.Error)]
    public async Task ShouldLogControllerFailureReasonAsync(
        int statusCode,
        LogLevel expectedLevel)
    {
        // Given
        Mock<ILoggingBroker> loggingBroker = new();
        DefaultHttpContext httpContext = new();

        ObjectResult result = new(value: "failure reason")
        {
            StatusCode = statusCode
        };

        ActionContext actionContext = new(
            httpContext: httpContext,
            routeData: new RouteData(),
            actionDescriptor: new ActionDescriptor());

        ResultExecutingContext executingContext = new(
            actionContext: actionContext,
            filters: new List<IFilterMetadata>(),
            result: result,
            controller: new object());

        ControllerFailureLoggingFilter filter = new(
            loggingBroker: loggingBroker.Object);

        // When
        await filter.OnResultExecutionAsync(
            context: executingContext,
            next: () =>
            {
                httpContext.Response.StatusCode = statusCode;

                return Task.FromResult(
                    result: new ResultExecutedContext(
                        actionContext: actionContext,
                        filters: new List<IFilterMetadata>(),
                        result: result,
                        controller: new object()));
            });

        // Then
        if (expectedLevel == LogLevel.Error)
        {
            loggingBroker.Verify(
                expression: value => value.LogError(
                    message: It.Is<string>(match: message =>
                        message.Contains(value: "failure reason")),
                    args: It.IsAny<object[]>()),
                times: Times.Once);
        }
        else
        {
            loggingBroker.Verify(
                expression: value => value.LogWarning(
                    message: It.Is<string>(match: message =>
                        message.Contains(value: "failure reason")),
                    args: It.IsAny<object[]>()),
                times: Times.Once);
        }
    }
}