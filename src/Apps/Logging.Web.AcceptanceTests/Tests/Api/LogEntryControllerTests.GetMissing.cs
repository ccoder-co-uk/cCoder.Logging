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
    public async Task ShouldReturnNotFoundWhenLogEntryDoesNotExist()
    {
        // Given
        const int missingLogEntryId = int.MaxValue;

        // When
        using HttpResponseMessage response = await Client.GetAsync(
            requestUri: $"{LogEntryRoute}({missingLogEntryId})");

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.NotFound);
    }
}