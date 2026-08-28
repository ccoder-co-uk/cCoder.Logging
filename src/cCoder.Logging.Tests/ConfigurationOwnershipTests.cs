// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Logging.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace cCoder.Logging.Tests;

public sealed partial class ConfigurationOwnershipTests
{
    [Fact]
    public void LoggingConfiguration_ShouldNotOwnPersistenceConfiguration()
    {
        // Given
        Type configurationType = typeof(LoggingConfiguration);

        // When
        string[] propertyNames = configurationType
            .GetProperties()
            .Select(selector: property => property.Name)
            .ToArray();

        // Then
        propertyNames.Should()
            .NotContain(unexpected: [
                "ConnectionString",
                "DebugInfo",
                "LogSQL"]);
    }

    [Fact]
    public void AddLoggingWeb_ShouldNotRegisterCoreDataServices()
    {
        // Given
        IServiceCollection services = new ServiceCollection();
        LoggingConfiguration configuration = new();

        typeof(LoggingConfiguration)
            .GetProperty(name: "ConnectionString")
            ?.SetValue(obj: configuration, value: "Server=(local);");

        // When
        services.AddLoggingWeb(configuration: configuration);

        // Then
        services.Should()
            .NotContain(predicate: descriptor =>
                descriptor.ServiceType == typeof(CoreDataContext));
    }
}