// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using FluentAssertions;
using Xunit;

namespace Logging.HostedServices.AcceptanceTests.Tests.Api;

public sealed partial class HealthTests
{
    [Fact]
    public async Task ShouldReturnOkWhenGetHealth()
    {
        // Given

        // When
        string response = await Client.GetStringAsync(requestUri: "/Health");

        // Then

        response.Should()
            .Contain(expected: "OK");
    }
}