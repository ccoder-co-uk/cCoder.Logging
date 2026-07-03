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
        string response = await Client.GetStringAsync("/Health");

        // Then
        response.Should().Contain("OK");
    }
}
