// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Logging.Web.AcceptanceTests.Tests.Api;

public sealed partial class HealthTests
{
    [Fact]
    public async Task ShouldReturnOkWhenGetHealth()
    {
        // When
        using HttpResponseMessage response = await Client.GetAsync("/Health");
        string content = await response.Content.ReadAsStringAsync();

        // Then
        response.IsSuccessStatusCode.Should().BeTrue();
        content.Should().Contain("OK");
    }
}