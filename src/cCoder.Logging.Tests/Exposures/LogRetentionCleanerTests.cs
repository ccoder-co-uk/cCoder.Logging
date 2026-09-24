// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Dependencies.HostedServices;
using cCoder.Logging.Exposures.HostedServices;
using cCoder.Logging.Services.Processings;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace cCoder.Logging.Tests.Exposures;

public sealed partial class LogRetentionCleanerTests
{
    [Fact]
    public async Task LogRetentionCleaner_WhenProcessingServiceIsScoped_CanBeValidatedByTheRootProvider()
    {
        // Given
        IServiceCollection services = new ServiceCollection();

        services.AddScoped<ILogEntryRetentionProcessingService, TestRetentionProcessingService>();

        services.AddSingleton<LogRetentionRunner>(
            implementationFactory: provider =>
                async cancellationToken =>
                {
                    using IServiceScope scope = provider.CreateScope();

                    ILogEntryRetentionProcessingService processingService =
                        scope.ServiceProvider.GetRequiredService<ILogEntryRetentionProcessingService>();

                    await processingService.RunLogRetentionAsync(
                        cancellationToken: cancellationToken);
                });

        services.AddSingleton<ILogRetentionCleaner, LogRetentionCleaner>();

        services.AddSingleton<IHostedService>(
            implementationFactory: provider =>
                provider.GetRequiredService<ILogRetentionCleaner>());

        // When
        using ServiceProvider provider = services.BuildServiceProvider(
            options: new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });

        // Then
        IHostedService hostedService = provider.GetRequiredService<IHostedService>();

        hostedService
            .Should()
            .BeOfType<LogRetentionCleaner>();

        await hostedService.StartAsync(
            cancellationToken: CancellationToken.None);
    }

    private sealed class TestRetentionProcessingService
        : ILogEntryRetentionProcessingService
    {
        public Task RunLogRetentionAsync(CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public ValueTask<int> DeleteExpiredLogEntriesAsync(
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(result: 0);
    }
}