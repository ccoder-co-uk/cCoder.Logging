// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using FluentAssertions;
using System.Net.Http.Json;
using Xunit;

namespace Logging.Web.AcceptanceTests.Tests.Api;

public sealed partial class LogEntryControllerTests
{
    [Fact]
    public async Task ShouldCreateLogEntryWhenPost()
    {
        // Given
        LogEntry expectedLogEntry = CreateLogEntry();

        // When
        using HttpResponseMessage response = await Client.PostAsJsonAsync(LogEntryRoute, expectedLogEntry);

        // Then
        response.IsSuccessStatusCode.Should().BeTrue();
        LogEntry storedLogEntry = await FindLogEntryAsync(expectedLogEntry.Message);
        storedLogEntry.Should().NotBeNull();
        storedLogEntry.AppId.Should().Be(1);
    }
}