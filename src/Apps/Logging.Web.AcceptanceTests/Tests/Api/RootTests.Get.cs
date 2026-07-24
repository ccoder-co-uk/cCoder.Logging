// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Logging.Web.AcceptanceTests.Tests.Api;

public sealed partial class RootTests
{
    [Fact]
    public async Task ShouldReturnToolsPageWhenGetRoot()
    {
        // Given

        // When
        string response = await Client.GetStringAsync(requestUri: "/");

        // Then

        response.Should()
            .Contain(expected: "/tools/index.html");
    }
}