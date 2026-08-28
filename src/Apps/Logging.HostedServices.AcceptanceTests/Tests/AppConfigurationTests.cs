// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Eventing.Models;
using cCoder.Logging.Models;
using Xunit;

namespace Logging.HostedServices.AcceptanceTests.Tests;

public sealed partial class AppConfigurationTests
{
    [Fact]
    public void ShouldExposeEveryRequiredDomainConfiguration()
    {
        // Given
        const string typeName =
            "Logging.HostedServices.Models.AppConfiguration, Logging.HostedServices";

        // When
        Type configurationType = Type.GetType(typeName: typeName);

        // Then
        Assert.NotNull(@object: configurationType);

        Assert.Equal(
            expected: typeof(CoreDataConfiguration),
            actual: configurationType.GetProperty(name: "CoreData")?.PropertyType);

        Assert.Equal(
            expected: typeof(LoggingConfiguration),
            actual: configurationType.GetProperty(name: "Logging")?.PropertyType);

        Assert.Equal(
            expected: typeof(EventingConfiguration),
            actual: configurationType.GetProperty(name: "Eventing")?.PropertyType);
    }
}