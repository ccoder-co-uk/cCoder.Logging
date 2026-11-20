// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005
#pragma warning disable STXFORMAT002
#pragma warning disable STXFORMAT008
#pragma warning disable STXFORMAT009
#pragma warning disable STXTEST005

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Authentication;
using System.Threading.Tasks;
using cCoder.Data.Models.Logging;
using cCoder.Logging.Brokers.Loggings;
using cCoder.Logging.Exposures.Controllers;
using cCoder.Logging.Models.Exceptions;
using cCoder.Logging.Services.Orchestrations;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace cCoder.Logging.Tests;

public sealed partial class ControllerResponseContractTests
{
    [Theory]
    [InlineData("entry-get-all", StatusCodes.Status200OK)]
    [InlineData("entry-get", StatusCodes.Status200OK)]
    [InlineData("entry-post", StatusCodes.Status201Created)]
    [InlineData("data-get-all", StatusCodes.Status200OK)]
    [InlineData("data-get", StatusCodes.Status200OK)]
    [InlineData("data-post", StatusCodes.Status201Created)]
    public async Task ShouldReturnSuccessAndLogInformationOnceAsync(
        string action,
        int expectedStatus)
    {
        ControllerFixture fixture = new();
        fixture.SetupSuccess(action);

        IActionResult result = await fixture.InvokeAsync(action);

        GetStatusCode(result).Should().Be(expectedStatus);
        fixture.LoggingBroker.Verify(broker => broker.LogInformation(
            It.IsAny<string>(),
            It.IsAny<object[]>()), Times.Once);
        fixture.VerifyNoFailureLogs();
    }

    [Theory]
    [MemberData(nameof(FailureCases))]
    public async Task ShouldClassifyFailureAndLogExactlyOnceAsync(
        string action,
        Exception exception,
        int expectedStatus)
    {
        ControllerFixture fixture = new();
        fixture.SetupFailure(action, exception);

        IActionResult result = await fixture.InvokeAsync(action);

        GetStatusCode(result).Should().Be(expectedStatus);

        if (expectedStatus == StatusCodes.Status500InternalServerError)
        {
            fixture.LoggingBroker.Verify(broker => broker.LogError(
                exception,
                It.IsAny<string>(),
                It.IsAny<object[]>()), Times.Once);
            fixture.LoggingBroker.Verify(broker => broker.LogWarning(
                It.IsAny<Exception>(),
                It.IsAny<string>(),
                It.IsAny<object[]>()), Times.Never);

            result.Should().BeOfType<ObjectResult>()
                .Which.Value.Should().NotBe(exception.Message);
        }
        else
        {
            fixture.LoggingBroker.Verify(broker => broker.LogWarning(
                exception,
                It.IsAny<string>(),
                It.IsAny<object[]>()), Times.Once);
            fixture.LoggingBroker.Verify(broker => broker.LogError(
                It.IsAny<Exception>(),
                It.IsAny<string>(),
                It.IsAny<object[]>()), Times.Never);
        }

        if (expectedStatus == StatusCodes.Status401Unauthorized)
        {
            result.Should().BeOfType<UnauthorizedObjectResult>()
                .Which.Value.Should().Be("Authentication failed.");
        }

        fixture.LoggingBroker.Verify(broker => broker.LogInformation(
            It.IsAny<string>(),
            It.IsAny<object[]>()), Times.Never);
    }

    [Theory]
    [InlineData("entry-post")]
    [InlineData("data-post")]
    public async Task ShouldWarnExactlyOnceWhenModelStateIsInvalidAsync(string action)
    {
        ControllerFixture fixture = new();
        fixture.AddModelError(action);

        IActionResult result = await fixture.InvokeAsync(action);

        result.Should().BeOfType<BadRequestObjectResult>();
        fixture.VerifyOneDirectWarning();
    }

    [Theory]
    [InlineData("entry-get")]
    [InlineData("data-get")]
    public async Task ShouldWarnExactlyOnceWhenItemIsMissingAsync(string action)
    {
        ControllerFixture fixture = new();

        IActionResult result = await fixture.InvokeAsync(action);

        result.Should().BeOfType<NotFoundResult>();
        fixture.VerifyOneDirectWarning();
    }

    public static IEnumerable<object[]> FailureCases()
    {
        string[] actions =
        [
            "entry-get-all",
            "entry-get",
            "entry-post",
            "data-get-all",
            "data-get",
            "data-post"
        ];

        foreach (string action in actions)
        {
            yield return
            [
                action,
                new LoggingValidationException(new ArgumentException()),
                StatusCodes.Status400BadRequest
            ];
            yield return
            [
                action,
                new AuthenticationException("Authentication failed."),
                StatusCodes.Status401Unauthorized
            ];
            yield return
            [
                action,
                new SecurityException("Forbidden."),
                StatusCodes.Status403Forbidden
            ];
            yield return
            [
                action,
                new InvalidOperationException("Database unavailable."),
                StatusCodes.Status500InternalServerError
            ];
        }
    }

    private static int? GetStatusCode(IActionResult result) => result switch
    {
        ObjectResult objectResult => objectResult.StatusCode,
        StatusCodeResult statusCodeResult => statusCodeResult.StatusCode,
        _ => null
    };

    private sealed class ControllerFixture
    {
        internal Mock<ILoggingBroker> LoggingBroker { get; } = new();
        private Mock<ILogEntryOrchestrationService> LogEntryService { get; } = new();
        private Mock<ILogDataItemOrchestrationService> LogDataItemService { get; } = new();
        private LogEntryController LogEntryController { get; }
        private LogDataItemController LogDataItemController { get; }

        internal ControllerFixture()
        {
            LogEntryController = new(
                LogEntryService.Object,
                LoggingBroker.Object);
            LogDataItemController = new(
                LogDataItemService.Object,
                LoggingBroker.Object);
        }

        internal void SetupSuccess(string action)
        {
            if (action == "entry-get-all")
            {
                LogEntryService.Setup(service => service.GetAllLogEntries(false))
                    .Returns(new[] { new LogEntry { Id = 1 } }.AsQueryable());
            }
            else if (action == "entry-get")
            {
                LogEntryService.Setup(service => service.GetLogEntry(1))
                    .Returns(new LogEntry { Id = 1 });
            }
            else if (action == "entry-post")
            {
                LogEntryService.Setup(service => service.AddLogEntryAsync(
                        It.IsAny<LogEntry>()))
                    .ReturnsAsync(new LogEntry { Id = 1 });
            }
            else if (action == "data-get-all")
            {
                LogDataItemService.Setup(service => service.GetAllLogDataItems(false))
                    .Returns(new[] { new LogDataItem { Id = 1 } }.AsQueryable());
            }
            else if (action == "data-get")
            {
                LogDataItemService.Setup(service => service.GetLogDataItem(1))
                    .Returns(new LogDataItem { Id = 1 });
            }
            else if (action == "data-post")
            {
                LogDataItemService.Setup(service => service.AddLogDataItemAsync(
                        It.IsAny<LogDataItem>()))
                    .ReturnsAsync(new LogDataItem { Id = 1 });
            }
        }

        internal void SetupFailure(string action, Exception exception)
        {
            if (action == "entry-get-all")
            {
                LogEntryService.Setup(service => service.GetAllLogEntries(false))
                    .Throws(exception);
            }
            else if (action == "entry-get")
            {
                LogEntryService.Setup(service => service.GetLogEntry(1))
                    .Throws(exception);
            }
            else if (action == "entry-post")
            {
                LogEntryService.Setup(service => service.AddLogEntryAsync(
                        It.IsAny<LogEntry>()))
                    .Throws(exception);
            }
            else if (action == "data-get-all")
            {
                LogDataItemService.Setup(service => service.GetAllLogDataItems(false))
                    .Throws(exception);
            }
            else if (action == "data-get")
            {
                LogDataItemService.Setup(service => service.GetLogDataItem(1))
                    .Throws(exception);
            }
            else if (action == "data-post")
            {
                LogDataItemService.Setup(service => service.AddLogDataItemAsync(
                        It.IsAny<LogDataItem>()))
                    .Throws(exception);
            }
        }

        internal void AddModelError(string action)
        {
            if (action == "entry-post")
            {
                LogEntryController.ModelState.AddModelError("Message", "Required");
            }
            else
            {
                LogDataItemController.ModelState.AddModelError("Key", "Required");
            }
        }

        internal async Task<IActionResult> InvokeAsync(string action) => action switch
        {
            "entry-get-all" => LogEntryController.Get(),
            "entry-get" => LogEntryController.Get(key: 1),
            "entry-post" => await LogEntryController.Post(new LogEntry()),
            "data-get-all" => LogDataItemController.Get(),
            "data-get" => LogDataItemController.Get(key: 1),
            "data-post" => await LogDataItemController.Post(new LogDataItem()),
            _ => throw new ArgumentOutOfRangeException(nameof(action))
        };

        internal void VerifyNoFailureLogs()
        {
            LoggingBroker.Verify(broker => broker.LogWarning(
                It.IsAny<string>(),
                It.IsAny<object[]>()), Times.Never);
            LoggingBroker.Verify(broker => broker.LogWarning(
                It.IsAny<Exception>(),
                It.IsAny<string>(),
                It.IsAny<object[]>()), Times.Never);
            LoggingBroker.Verify(broker => broker.LogError(
                It.IsAny<Exception>(),
                It.IsAny<string>(),
                It.IsAny<object[]>()), Times.Never);
        }

        internal void VerifyOneDirectWarning()
        {
            LoggingBroker.Verify(broker => broker.LogWarning(
                It.IsAny<string>(),
                It.IsAny<object[]>()), Times.Once);
            LoggingBroker.Verify(broker => broker.LogWarning(
                It.IsAny<Exception>(),
                It.IsAny<string>(),
                It.IsAny<object[]>()), Times.Never);
            LoggingBroker.Verify(broker => broker.LogInformation(
                It.IsAny<string>(),
                It.IsAny<object[]>()), Times.Never);
            LoggingBroker.Verify(broker => broker.LogError(
                It.IsAny<Exception>(),
                It.IsAny<string>(),
                It.IsAny<object[]>()), Times.Never);
        }
    }
}