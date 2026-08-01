// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using System.Net;
using Xunit;

namespace Logging.Web.AcceptanceTests.Tests.Api;

public sealed partial class LogEntryControllerTests
{
    [Fact]
    public async Task ShouldReturnOkWhenGetLogEntries()
    {
        // Given

        // When
        using HttpResponseMessage response = await Client.GetAsync(
            requestUri: LogEntryRoute);

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK);
    }
}