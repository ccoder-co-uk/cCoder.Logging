// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Logging.Web.AcceptanceTests.Tests.Api;

public sealed partial class AuthenticationTests
{
    [Fact]
    public async Task ShouldLoginThroughSecurityAccountApi()
    {
        // Given
        cCoder.Security.Models.DTOs.RegisterUser user = await RegisterUserAsync();

        // When
        cCoder.Security.Models.Entities.Token token = await LoginAsync(user: user);

        // Then

        token.Should()
            .NotBeNull();

        token.Id.Should()
            .NotBeNullOrWhiteSpace();

        token.UserName.Should()
            .NotBeNullOrWhiteSpace();

        token.Expires.Should()
            .BeAfter(expected: DateTimeOffset.UtcNow);
    }
}