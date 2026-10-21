// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;

using cCoder.Logging.Brokers;
using cCoder.Logging.Models;
using cCoder.Logging.Services.Orchestrations;
using cCoder.Security.Models.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using RequestCoordinator = cCoder.Logging.RequestLoggingCoordinator;
using QueueCoordinator = cCoder.Logging.RequestLogQueueCoordinator;

namespace cCoder.Core.Services.Tests.Logging;

public sealed partial class RequestLoggingCoordinatorTests
{
    [Fact]
    public async Task ShouldDispatchQueuedCaptureThroughOrchestrationAsync()
    {
        // Given
        LogEntryCaptureRequest request = new()
        {
            Message = "Captured request"
        };

        Mock<ILogEntryCaptureQueue> queueMock = new(
            behavior: MockBehavior.Strict);

        queueMock
            .Setup(expression: queue => queue.ReadAllAsync())
            .Returns(value: ReadOneAsync(request: request));

        Mock<ILogEntryCaptureOrchestrationService> orchestrationServiceMock =
            new(behavior: MockBehavior.Strict);

        orchestrationServiceMock
            .Setup(expression: service =>
                service.CaptureLogEntryCaptureRequestAsync(
                    logEntryCaptureRequest: request))
            .Returns(value: ValueTask.CompletedTask);

        ServiceProvider serviceProvider = new ServiceCollection()
            .AddSingleton(implementationInstance: orchestrationServiceMock.Object)
            .BuildServiceProvider();

        QueueCoordinator coordinator = new(
            queue: queueMock.Object,
            serviceScopeFactory: serviceProvider.GetRequiredService<IServiceScopeFactory>(),
            logger: Mock.Of<ILogger<QueueCoordinator>>());

        // When
        await coordinator.RunAsync();

        // Then
        queueMock.VerifyAll();
        orchestrationServiceMock.VerifyAll();
    }

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
                logEntryCaptureRequest: It.Is<LogEntryCaptureRequest>(match: request =>
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

    private static async IAsyncEnumerable<LogEntryCaptureRequest> ReadOneAsync(
        LogEntryCaptureRequest request)
    {
        yield return request;
        await Task.CompletedTask;
    }
}