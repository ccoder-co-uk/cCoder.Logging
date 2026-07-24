// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

        actualStatusCode.Should()
            .NotBe(unexpected: (int)HttpStatusCode.NotFound);

        actualStatusCode.Should()
            .NotBe(unexpected: (int)HttpStatusCode.InternalServerError);
    }
}