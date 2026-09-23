// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.CodeAnalysis.Exposures;
using cCoder.Logging.Brokers.Loggings;
using Xunit;

namespace cCoder.Logging.Tests;

public sealed partial class PackageRuntimeDependencyTests
{
    [Fact]
    public void LoggingAssembly_WhenMarkersAreUsed_ReferencesRuntimeContractsOnly()
    {
        // Given
        Assembly loggingAssembly = typeof(ILoggingBroker).Assembly;

        string contractsAssemblyName = typeof(IUtilityBroker)
            .Assembly
            .GetName()
            .Name;

        string[] referencedAssemblyNames = loggingAssembly
            .GetReferencedAssemblies()
            .Select(selector: assemblyName => assemblyName.Name)
            .ToArray();

        // When
        bool referencesRuntimeContracts = referencedAssemblyNames
            .Contains(value: contractsAssemblyName);

        bool referencesAnalyzerRuntime = referencedAssemblyNames
            .Contains(value: "cCoder.CodeAnalysis");

        // Then
        Assert.True(
            condition: referencesRuntimeContracts,
            userMessage: "Logging must reference the runtime marker contracts.");

        Assert.False(
            condition: referencesAnalyzerRuntime,
            userMessage: "Logging must not reference the analyzer runtime.");
    }
}