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
        string response = await Client.GetStringAsync("/");

        // Then
        response.Should().Contain("Logging Hosted Services");
        response.Should().Contain("LogRetentionCleaner");
    }
}