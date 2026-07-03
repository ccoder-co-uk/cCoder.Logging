using FluentAssertions;
using Xunit;

namespace Logging.Web.AcceptanceTests.Tests.Api;

public sealed partial class AuthenticationTests
{
    [Fact]
    public async Task ShouldLoginThroughSecurityAccountApi()
    {
        // Given
        cCoder.Security.Objects.DTOs.RegisterUser user = await RegisterUserAsync();

        // When
        cCoder.Security.Objects.Entities.Token token = await LoginAsync(user);

        // Then
        token.Should().NotBeNull();
        token.Id.Should().NotBeNullOrWhiteSpace();
        token.UserName.Should().NotBeNullOrWhiteSpace();
        token.Expires.Should().BeAfter(DateTimeOffset.UtcNow);
    }
}
