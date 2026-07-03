using System.Net;
using FluentAssertions;
using Xunit;


namespace Logging.Web.AcceptanceTests.Tests.Api;

public sealed partial class LogHubTests
{
    [Fact]
    public async Task ShouldReturnNonErrorResponseForNegotiate()
    {
        // Given

        // When
        int actualStatusCode = await NegotiateAsync();

        // Then
        actualStatusCode.Should().NotBe((int)HttpStatusCode.NotFound);
        actualStatusCode.Should().NotBe((int)HttpStatusCode.InternalServerError);
    }
}



