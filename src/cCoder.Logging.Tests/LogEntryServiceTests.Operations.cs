// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Logging;

public partial class LogEntryServiceTests
{
    [Fact]
    public void ShouldReturnLogEntryWhenGetLogEntry()
    {
        // Given
        LogEntry expectedLogEntry = CreateRandomLogEntry();

        logEntryBrokerMock
            .Setup(expression: broker => broker.SelectAllLogEntries())
            .Returns(value: new[] { expectedLogEntry }.AsQueryable());

        // When

        LogEntry actualLogEntry = logEntryService.GetLogEntry(
            logEntryId: expectedLogEntry.Id);

        // Then

        actualLogEntry.Id.Should()
            .Be(expected: expectedLogEntry.Id);

        logEntryBrokerMock.VerifyAll();
    }

    [Fact]
    public void ShouldReturnLogEntriesWhenGetAllLogEntries()
    {
        // Given
        IQueryable<LogEntry> expectedLogEntries =
            new[] { CreateRandomLogEntry() }.AsQueryable();

        logEntryBrokerMock
            .Setup(expression: broker => broker.SelectAllLogEntries())
            .Returns(value: expectedLogEntries);

        // When

        IQueryable<LogEntry> actualLogEntries =
            logEntryService.GetAllLogEntries();

        // Then

        actualLogEntries.Should()
            .BeSameAs(expected: expectedLogEntries);

        logEntryBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldAddLogEntryWhenAddLogEntryAsync()
    {
        // Given
        LogEntry newLogEntry = CreateRandomLogEntry(id: 0);
        newLogEntry.AppId = 7;

        User user = TestUsers.WithPrivilege(
            privilege: "LogEntry_create",
            appId: newLogEntry.AppId);

        authorizationBrokerMock
            .Setup(expression: broker => broker.SelectCurrentUser())
            .Returns(value: user);

        logEntryBrokerMock
            .Setup(expression: broker => broker.InsertLogEntryAsync(
newLogEntry: It.IsAny<LogEntry>()))
            .ReturnsAsync(valueFunction: (LogEntry logEntry) =>
            {
                logEntry.Id = 42;
                return logEntry;
            });

        // When

        LogEntry savedLogEntry = await logEntryService.AddLogEntryAsync(
            newLogEntry: newLogEntry);

        // Then

        savedLogEntry.Should()
            .BeSameAs(expected: newLogEntry);

        savedLogEntry.Id.Should()
            .Be(expected: 42);

        authorizationBrokerMock.VerifyAll();
        logEntryBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldAddSystemLogEntryWhenAddSystemLogEntryAsync()
    {
        // Given
        LogEntry newLogEntry = CreateRandomLogEntry(id: 0);

        logEntryBrokerMock
            .Setup(expression: broker => broker.InsertLogEntryAsync(
newLogEntry: It.IsAny<LogEntry>()))
            .ReturnsAsync(valueFunction: (LogEntry logEntry) =>
            {
                logEntry.Id = 42;
                return logEntry;
            });

        // When

        LogEntry savedLogEntry =
            await logEntryService.AddSystemLogEntryAsync(
                newLogEntry: newLogEntry);

        // Then

        savedLogEntry.Should()
            .BeSameAs(expected: newLogEntry);

        savedLogEntry.Id.Should()
            .Be(expected: 42);

        authorizationBrokerMock.VerifyNoOtherCalls();
        logEntryBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldUpdateLogEntryWhenUpdateLogEntryAsync()
    {
        // Given
        LogEntry updatedLogEntry = CreateRandomLogEntry();
        updatedLogEntry.AppId = 7;

        User user = TestUsers.WithPrivilege(
            privilege: "LogEntry_update",
            appId: updatedLogEntry.AppId);

        authorizationBrokerMock
            .Setup(expression: broker => broker.SelectCurrentUser())
            .Returns(value: user);

        logEntryBrokerMock
            .Setup(expression: broker => broker.UpdateLogEntryAsync(
updatedLogEntry: It.IsAny<LogEntry>()))
            .ReturnsAsync(valueFunction: (LogEntry logEntry) => logEntry);

        // When

        LogEntry savedLogEntry =
            await logEntryService.UpdateLogEntryAsync(
                updatedLogEntry: updatedLogEntry);

        // Then

        savedLogEntry.Should()
            .BeSameAs(expected: updatedLogEntry);

        authorizationBrokerMock.VerifyAll();
        logEntryBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldDeleteLogEntryWhenDeleteLogEntryAsync()
    {
        // Given
        LogEntry deletedLogEntry = CreateRandomLogEntry();
        deletedLogEntry.AppId = 7;

        User user = TestUsers.WithPrivilege(
            privilege: "LogEntry_delete",
            appId: deletedLogEntry.AppId);

        logEntryBrokerMock
            .Setup(expression: broker => broker.SelectAllLogEntries())
            .Returns(value: new[] { deletedLogEntry }.AsQueryable());

        authorizationBrokerMock
            .Setup(expression: broker => broker.SelectCurrentUser())
            .Returns(value: user);

        logEntryBrokerMock
            .Setup(expression: broker => broker.DeleteLogEntryAsync(
deletedLogEntry: It.IsAny<LogEntry>()))
            .ReturnsAsync(value: 1);

        // When

        await logEntryService.DeleteLogEntryAsync(
            logEntryId: deletedLogEntry.Id);

        // Then
        authorizationBrokerMock.VerifyAll();
        logEntryBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldDeleteLogEntriesWhenDeleteLogEntriesBeforeAsync()
    {
        // Given
        DateTime cutoff = DateTime.UtcNow;

        logEntryBrokerMock
            .Setup(expression: broker => broker.DeleteLogEntriesBeforeAsync(cutoff: cutoff))
            .ReturnsAsync(value: 3);

        // When

        int deletedCount =
            await logEntryService.DeleteLogEntriesBeforeAsync(
                cutoff: cutoff);

        // Then

        deletedCount.Should()
            .Be(expected: 3);

        logEntryBrokerMock.VerifyAll();
    }

    [Fact]
    public void ShouldReturnAppIdWhenResolveAppId()
    {
        // Given
        const string Domain = "app.local";
        const int ExpectedAppId = 7;

        logEntryBrokerMock
            .Setup(expression: broker => broker.SelectAppIdByDomainOrName(domainOrName: Domain))
            .Returns(value: ExpectedAppId);

        // When

        int? actualAppId = logEntryService.ResolveAppId(
            domainOrName: Domain);

        // Then

        actualAppId.Should()
            .Be(expected: ExpectedAppId);

        logEntryBrokerMock.VerifyAll();
    }
}