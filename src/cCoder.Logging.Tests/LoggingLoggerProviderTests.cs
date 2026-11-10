// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

using cCoder.Logging.Brokers;
using cCoder.Logging.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace cCoder.Logging.Tests;

public sealed partial class LoggingLoggerProviderTests
{
    [Fact]
    public void ShouldCaptureEveryLogLevelRegardlessOfHostMinimum()
    {
        // Given
        List<LogEntryCaptureRequest> capturedRequests = [];

        Mock<ILogEntryCaptureQueue> queueMock = new(
            behavior: MockBehavior.Strict);

        queueMock
            .Setup(expression: queue => queue.TryEnqueue(
                logEntryCaptureRequest:
                    It.IsAny<LogEntryCaptureRequest>()))
            .Callback<LogEntryCaptureRequest>(
                action: capturedRequests.Add)
            .Returns(value: true);

        ServiceCollection services = new();

        services.AddLogging(configure: builder =>
            builder.SetMinimumLevel(level: LogLevel.Warning));

        services.AddLoggingWeb(
            configuration: new LoggingConfiguration
            {
                DatabaseMinimumLogLevel = LogLevel.Warning,
                RequestLoggingQueueCapacity = 16,
            });

        services.AddSingleton<ILogEntryCaptureQueue>(
            implementationInstance: queueMock.Object);

        using ServiceProvider serviceProvider =
            services.BuildServiceProvider();

        ILogger logger = serviceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(categoryName: "Application.Tests");

        LogLevel[] expectedLevels =
        [
            LogLevel.Trace,
            LogLevel.Debug,
            LogLevel.Information,
            LogLevel.Warning,
            LogLevel.Error,
            LogLevel.Critical,
        ];

        // When
        foreach (LogLevel level in expectedLevels)
        {
            logger.Log(
                logLevel: level,
                eventId: default,
                state: level,
                exception: null,
                formatter: (state, _) => state.ToString());
        }

        // Then
        capturedRequests.Count
            .Should()
            .Be(expected: expectedLevels.Length);

        for (int index = 0; index < expectedLevels.Length; index++)
        {
            capturedRequests[index].Level
                .Should()
                .Be(expected: expectedLevels[index]);
        }

        capturedRequests
            .Where(predicate: request =>
                request.Level < LogLevel.Warning)
            .Should()
            .OnlyContain(predicate: request => !request.Persist);

        capturedRequests
            .Where(predicate: request =>
                request.Level >= LogLevel.Warning)
            .Should()
            .OnlyContain(predicate: request => request.Persist);

        queueMock.VerifyAll();
    }
}