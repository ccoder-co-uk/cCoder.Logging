// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Api.OData;
using cCoder.Logging.Brokers;
using cCoder.Logging.Exposures.HostedServices;
using cCoder.Logging.Exposures.Logging;
using cCoder.Logging.Models;
using cCoder.Logging.Services;
using cCoder.Logging.Services.Foundations;
using cCoder.Logging.Services.Foundations.Events;
using cCoder.Logging.Services.Orchestrations;
using cCoder.Logging.Services.Processings;
using cCoder.Eventing;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi;
using AuthorizationBroker = cCoder.Logging.Brokers.AuthorizationBroker;
using IAuthorizationBroker = cCoder.Logging.Brokers.IAuthorizationBroker;


namespace cCoder.Logging;

public static partial class IServiceCollectionExtensions
{
    public static void AddLoggingWeb(
        this IServiceCollection services,
        Action<LoggingConfiguration> configure = null,
        ODataConventionModelBuilder builder = null) =>
        services.AddConfiguredLoggingWeb((_, configuration) => configure?.Invoke(configuration), builder);

    public static void AddLoggingHostedServices(
        this IServiceCollection services,
        Action<LoggingConfiguration> configure = null)
    {
        services.AddConfiguredLogging((_, configuration) => configure?.Invoke(configuration));
        services.AddHostedServiceExposures();
    }

    private static void AddLogging(this IServiceCollection services)
    {
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddSingleton<ILoggerProvider, LoggingLoggerProvider>();
    }

    private static void AddLoggingWeb(this IServiceCollection services, ODataConventionModelBuilder builder = null)
    {
        services.AddLogging();

    }

    private static void AddEventingTypes(this IServiceCollection services)
    {
        services.AddEventingForType<LogDataItem>();
        services.AddEventingForType<LogEntry>();
    }

    private static void AddBrokers(this IServiceCollection services)
    {
        services.AddTransient<ILogDataItemEventBroker, LogDataItemEventBroker>();
        services.AddTransient<ILogEntryEventBroker, LogEntryEventBroker>();
        services.AddTransient<ILogDataItemBroker, LogDataItemBroker>();
        services.AddTransient<ILogEntryBroker, LogEntryBroker>();
        services.AddTransient<ILogEntryStreamBroker, LogEntryStreamBroker>();
        services.AddTransient<IAuthorizationBroker, AuthorizationBroker>();
    }

    private static void AddFoundations(this IServiceCollection services)
    {
        services.AddTransient<ILoggingMetadataTypeService, LoggingMetadataTypeService>();
        services.AddTransient<ILogDataItemService, LogDataItemService>();
        services.AddTransient<ILogEntryService, LogEntryService>();
        services.AddTransient<ILogDataItemEventService, LogDataItemEventService>();
        services.AddTransient<ILogEntryEventService, LogEntryEventService>();
    }

    private static void AddOrchestrations(this IServiceCollection services)
    {
        services.AddTransient<ILogDataItemOrchestrationService, LogDataItemOrchestrationService>();
        services.AddTransient<ILogEntryOrchestrationService, LogEntryOrchestrationService>();
        services.AddTransient<ILogEntryCaptureOrchestrationService, LogEntryCaptureOrchestrationService>();
        services.AddTransient<ILogRetentionOrchestrationService, LogRetentionOrchestrationService>();
    }

    private static void AddProcessings(this IServiceCollection services)
    {
        services.AddTransient<ILogDataItemEventProcessingService, LogDataItemEventProcessingService>();
        services.AddTransient<ILogDataItemProcessingService, LogDataItemProcessingService>();
        services.AddTransient<ILogEntryEventProcessingService, LogEntryEventProcessingService>();
        services.AddTransient<ILogEntryProcessingService, LogEntryProcessingService>();
    }

    private static void AddHostedServiceExposures(this IServiceCollection services)
    {
        services.AddSingleton<ILogRetentionCleaner, LogRetentionCleaner>();
        services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<ILogRetentionCleaner>());
    }
}