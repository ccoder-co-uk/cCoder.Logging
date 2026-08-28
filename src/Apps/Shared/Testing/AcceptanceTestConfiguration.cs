// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Data.SqlClient;

namespace Apps.Shared.Testing;

internal sealed class AcceptanceTestConfiguration
{
    private AcceptanceTestConfiguration(
        string coreConnectionString,
        string securityConnectionString,
        string securityDecryptionKey)
    {
        CoreConnectionString = coreConnectionString;
        SecurityConnectionString = securityConnectionString;
        SecurityDecryptionKey = securityDecryptionKey;
    }

    internal string CoreConnectionString { get; }
    internal string SecurityConnectionString { get; }
    internal string SecurityDecryptionKey { get; }

    internal static AcceptanceTestConfiguration Load()
    {
        string runSuffix = $"-acceptance-{Guid.NewGuid():N}";

        return new AcceptanceTestConfiguration(
            coreConnectionString: AddDatabaseSuffix(
                connectionString: ReadRequiredValue(
                    variableName: "CoreData__ConnectionString"),
                suffix: runSuffix),
            securityConnectionString: AddDatabaseSuffix(
                connectionString: ReadRequiredValue(
                    variableName: "SecurityData__ConnectionString"),
                suffix: runSuffix),
            securityDecryptionKey: ReadRequiredValue(
                variableName: "Security__DecryptionKey"));
    }

    private static string AddDatabaseSuffix(
        string connectionString,
        string suffix)
    {
        SqlConnectionStringBuilder builder =
            new(connectionString: connectionString)
            {
                Encrypt = true,
                TrustServerCertificate = true
            };

        string databaseName = builder.InitialCatalog ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value: databaseName))
        {
            throw new InvalidOperationException(
                "Acceptance test connection strings must name a database.");
        }

        builder.InitialCatalog = $"{databaseName}{suffix}";
        return builder.ConnectionString;
    }

    private static string ReadRequiredValue(string variableName)
    {
        string value =
            Environment.GetEnvironmentVariable(variable: variableName)
            ?? Environment.GetEnvironmentVariable(
                variable: variableName,
                target: EnvironmentVariableTarget.User)
            ?? Environment.GetEnvironmentVariable(
                variable: variableName,
                target: EnvironmentVariableTarget.Machine);

        if (!string.IsNullOrWhiteSpace(value: value))
        {
            return value;
        }

        throw new InvalidOperationException(
            $"Required configuration environment variable '{variableName}' was not found.");
    }
}