// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Logging.HostedServices.AcceptanceTests.Tests.Api;

public sealed partial class HostedServicesReportTests
{
    [Fact]
    public async Task ShouldReturnHostedServicesReportWhenGetRoot()
    {
        // Given

        // When
        string response = await Client.GetStringAsync(requestUri: "/");

        // Then

        response.Should()
            .Contain(expected: "Logging Hosted Services");

        response.Should()
            .Contain(expected: "LogRetentionCleaner");
    }
}