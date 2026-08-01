// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using System.Net;
using Xunit;

namespace Logging.Web.AcceptanceTests.Tests.Api;

public sealed partial class LogDataItemControllerTests
{
    [Fact]
    public async Task ShouldReturnNotFoundWhenLogDataItemDoesNotExist()
    {
        // Given
        const int missingLogDataItemId = int.MaxValue;

        // When
        using HttpResponseMessage response = await Client.GetAsync(
            requestUri: $"{LogDataItemRoute}({missingLogDataItemId})");

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.NotFound);
    }
}