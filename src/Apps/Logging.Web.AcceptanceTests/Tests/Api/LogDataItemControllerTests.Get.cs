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
    public async Task ShouldReturnOkWhenGetLogDataItems()
    {
        // Given

        // When
        using HttpResponseMessage response = await Client.GetAsync(
            requestUri: LogDataItemRoute);

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK);
    }
}