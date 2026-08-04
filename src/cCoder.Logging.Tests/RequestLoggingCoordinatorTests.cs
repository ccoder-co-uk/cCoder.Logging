// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Brokers;
using cCoder.Logging.Models;
using cCoder.Security.Models.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using RequestCoordinator = cCoder.Logging.RequestLoggingCoordinator;

namespace cCoder.Core.Services.Tests.Logging;

public sealed partial class RequestLoggingCoordinatorTests
{
    [Fact]
    public async Task ShouldSnapshotAuthoritativeRequestContextAsync()
    {
        // Given
        Mock<ILogEntryCaptureQueue> queueMock = new(
            behavior: MockBehavior.Strict);

        Mock<ISession> sessionMock = new(
            behavior: MockBehavior.Strict);

        sessionMock
            .SetupGet(expression: session => session.Id)
            .Returns(value: "session-9");

        Mock<ISessionFeature> sessionFeatureMock = new(
            behavior: MockBehavior.Strict);

        sessionFeatureMock
            .SetupGet(expression: feature => feature.Session)
            .Returns(value: sessionMock.Object);

        queueMock
            .Setup(expression: queue => queue.TryEnqueue(
                request: It.Is<LogEntryCaptureRequest>(match: request =>
                    request.Url == "https://example.test/orders?status=open"
                    && request.UserId == "user-7"
                    && request.SessionId == "session-9"
                    && request.RequestDomain == "example.test")))
            .Returns(value: true);

        ServiceProvider requestServices = new ServiceCollection()
            .AddSingleton<ISSOAuthInfo>(
                implementationInstance: new SSOAuthInfo
                {
                    SSOUserId = "user-7"
                })
            .BuildServiceProvider();

        DefaultHttpContext context = new()
        {
            RequestServices = requestServices
        };

        context.Request.Scheme = "https";
        context.Request.Host = new HostString(value: "example.test");
        context.Request.Path = "/orders";
        context.Request.QueryString = new QueryString(value: "?status=open");
        context.Features.Set(instance: sessionFeatureMock.Object);

        RequestCoordinator coordinator = new(
            queue: queueMock.Object,
            configuration: new LoggingConfiguration
            {
                RequestLoggingEnabled = true
            },
            logger: Mock.Of<ILogger<RequestCoordinator>>());

        // When
        await coordinator.CaptureRequestAsync(
            context: context,
            next: _ => Task.CompletedTask);

        // Then
        queueMock.VerifyAll();
        sessionMock.VerifyAll();
        sessionFeatureMock.VerifyAll();
    }
}