// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Extensions.OData;
using cCoder.Logging.Brokers;
using cCoder.Logging.Exposures.HostedServices;
using cCoder.Logging.Dependencies.Logging;
using cCoder.Logging.Models;
using cCoder.Logging.Brokers.OData;
using cCoder.Logging.Models.OData;
using cCoder.Logging.Exposures;
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
using System.Threading.Channels;
using AuthorizationBroker = cCoder.Logging.Brokers.AuthorizationBroker;
using IAuthorizationBroker = cCoder.Logging.Brokers.IAuthorizationBroker;


namespace cCoder.Logging;

public static partial class IServiceCollectionExtensions
{
    public static void AddLoggingWeb(
        this IServiceCollection services,
        Action<LoggingConfiguration> configure = null,
        ODataConventionModelBuilder builder = null)
    {
        LoggingConfiguration configuration = services.CreateLoggingConfiguration();
        configure?.Invoke(obj: configuration);
        services.AddLoggingWeb(configuration: configuration, builder: builder);
    }

    public static void AddLoggingWeb(
        this IServiceCollection services,
        LoggingConfiguration configuration,
        ODataConventionModelBuilder builder = null)
    {
        services.RegisterLoggingConfiguration(
            configuration: configuration);
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddExposures();
        services.AddLoggingApi(
            configuration: configuration,
            builder: builder);
    }

    public static void AddLoggingHostedServices(
        this IServiceCollection services,
        Action<LoggingConfiguration> configure = null)
    {
        LoggingConfiguration configuration = services.CreateLoggingConfiguration();
        configure?.Invoke(obj: configuration);
        services.AddLoggingHostedServices(configuration: configuration);
    }

    public static void AddLoggingHostedServices(
        this IServiceCollection services,
        LoggingConfiguration configuration)
    {
        services.RegisterLoggingConfiguration(
            configuration: configuration);
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddExposures();
        services.AddHostedServiceExposures();
    }

    private static void AddExposures(
        this IServiceCollection services)
    {
        services.AddSingleton<ILogEntryCaptureQueue>(
            implementationFactory: provider =>
            {
                LoggingConfiguration configuration =
                    provider.GetRequiredService<LoggingConfiguration>();

                BoundedChannelFullMode fullMode =
                    configuration.RequestLoggingQueueFullBehavior == RequestLoggingQueueFullBehavior.DropOldest
                        ? BoundedChannelFullMode.DropOldest
                        : BoundedChannelFullMode.Wait;

                Channel<LogEntryCaptureRequest> channel =
                    Channel.CreateBounded<LogEntryCaptureRequest>(
                        options: new BoundedChannelOptions(configuration.RequestLoggingQueueCapacity)
                        {
                            AllowSynchronousContinuations = false,
                            FullMode = fullMode,
                            SingleReader = true,
                            SingleWriter = false
                        });

                return new LogEntryCaptureQueue(channel: channel);
            });
        services.AddSingleton<IRequestLoggingCoordinator, RequestLoggingCoordinator>();
        services.AddSingleton<IRequestLogQueueCoordinator, RequestLogQueueCoordinator>();
        services.AddTransient<RequestLoggingMiddleware>();
        services.AddHostedService<LogEntryCaptureWorker>();
        services.AddTransient<ILogDataItemManager, LogDataItemManager>();
        services.AddTransient<ILogEntryManager, LogEntryManager>();
        services.AddSingleton<ILoggerProvider, LoggingLoggerProvider>();
    }

    private static void AddEventingTypes(this IServiceCollection services)
    {
        services.AddEventingForType<LogDataItem>();
        services.AddEventingForType<LogEntry>();
    }

    private static void AddBrokers(this IServiceCollection services)
    {
        services.AddTransient<Brokers.Loggings.ILoggingBroker, Brokers.Loggings.LoggingBroker>();
        services.AddTransient<IAuthInfoBroker, AuthInfoBroker>();
        services.AddTransient<ILogDataItemEventBroker, LogDataItemEventBroker>();
        services.AddTransient<ILogEntryEventBroker, LogEntryEventBroker>();
        services.AddTransient<ILogDataItemBroker, LogDataItemBroker>();
        services.AddTransient<ILogEntryBroker, LogEntryBroker>();
        services.AddTransient<ILogEntryStreamBroker, LogEntryStreamBroker>();
        services.AddTransient<ILogHubBroker, LogHubBroker>();
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
    }

    private static void AddProcessings(this IServiceCollection services)
    {
        services.AddTransient<ILogDataItemEventProcessingService, LogDataItemEventProcessingService>();
        services.AddTransient<ILogDataItemProcessingService, LogDataItemProcessingService>();
        services.AddTransient<ILogEntryEventProcessingService, LogEntryEventProcessingService>();
        services.AddTransient<ILogEntryCaptureProcessingService, LogEntryCaptureProcessingService>();
        services.AddTransient<ILogEntryProcessingService, LogEntryProcessingService>();
        services.AddTransient<
            ILogHubProcessingService,
            LogHubProcessingService>();
        services.AddTransient<
            ILogEntryRetentionProcessingService,
            LogEntryRetentionProcessingService>();
    }

    private static void AddHostedServiceExposures(this IServiceCollection services)
    {
        services.AddSingleton<ILogRetentionCleaner, LogRetentionCleaner>();
        services.AddSingleton<IHostedService>(
            implementationFactory: provider =>
                provider.GetRequiredService<ILogRetentionCleaner>());
    }

    private static LoggingConfiguration CreateLoggingConfiguration(
        this IServiceCollection services) =>
        new()
        {
            ConnectionString = string.Empty,
            StreamLogEntries = true,
            RetentionDays = 30,
            RetentionIntervalMinutes = 60,
            RootPath = "Api/Logging",
            RequestLoggingEnabled = true,
            RequestLoggingQueueCapacity = 1024,
            RequestLoggingQueueFullBehavior = RequestLoggingQueueFullBehavior.DropNewest,
            DatabaseMinimumLogLevel = LogLevel.Warning,
            EventProviders = []
        };
}