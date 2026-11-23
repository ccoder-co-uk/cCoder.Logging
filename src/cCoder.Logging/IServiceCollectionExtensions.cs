// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.Metrics;
using System.Threading.Channels;

using cCoder.Data.Models.Logging;
using cCoder.Logging.Extensions.OData;
using cCoder.Logging.Brokers;
using cCoder.Logging.Exposures.HostedServices;
using cCoder.Logging.Dependencies.HostedServices;
using cCoder.Logging.Dependencies.Authentication;
using cCoder.Logging.Dependencies.Logging;
using cCoder.Logging.Models;
using cCoder.Logging.Brokers.OData;
using cCoder.Logging.Brokers.Metadata;
using cCoder.Logging.Brokers.Loggings;
using cCoder.Logging.Models.OData;
using cCoder.Logging.Exposures;
using cCoder.Logging.Exposures.Hubs;
using cCoder.Logging.Services.Foundations;
using cCoder.Logging.Services.Foundations.Events;
using cCoder.Logging.Services.Orchestrations;
using cCoder.Logging.Services.Processings;
using cCoder.Eventing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
        services.AddLogging(configure: builder =>
            builder.AddFilter<LoggingLoggerProvider>(
                levelFilter: level => level != LogLevel.None));
        services.AddHttpContextAccessor();
        services.AddMetrics();
        ISignalRServerBuilder signalR = services.AddSignalR();
        signalR.AddHubOptions<LogHub>(configure: options =>
            options.AddFilter<LogHubAuthenticationDependency>());

        services.AddSingleton<ILogEntryCaptureQueue>(
            implementationFactory: provider =>
            {
                LoggingConfiguration configuration =
                    provider.GetRequiredService<LoggingConfiguration>();

                bool dropsOldest = configuration.RequestLoggingQueueFullBehavior
                    == RequestLoggingQueueFullBehavior.DropOldest;

                Meter meter = provider.GetRequiredService<IMeterFactory>()
                    .Create(name: "cCoder.Logging");
                Counter<long> droppedEntries = meter.CreateCounter<long>(
                    name: "ccoder.logging.capture.dropped",
                    unit: "{entry}");
                string dropReason = dropsOldest ? "drop_oldest" : "drop_newest";

                Channel<LogEntryCaptureRequest> channel =
                    Channel.CreateBounded<LogEntryCaptureRequest>(
                        options: new BoundedChannelOptions(configuration.RequestLoggingQueueCapacity)
                        {
                            AllowSynchronousContinuations = false,
                            FullMode = dropsOldest
                                ? BoundedChannelFullMode.DropOldest
                                : BoundedChannelFullMode.DropWrite,
                            SingleReader = true,
                            SingleWriter = false
                        },
                        itemDropped: _ => droppedEntries.Add(
                            delta: 1,
                            tag: new("reason", dropReason)));

                return new LogEntryCaptureQueue(channel: channel);
            });
        services.AddSingleton<IRequestLoggingCoordinator, RequestLoggingCoordinator>();
        services.AddSingleton<IRequestLogQueueCoordinator, RequestLogQueueCoordinator>();
        services.AddTransient<RequestLoggingMiddleware>();
        services.AddScoped<ControllerFailureLoggingFilter>();
        services.Configure<MvcOptions>(configureOptions: options =>
            options.Filters.AddService<ControllerFailureLoggingFilter>());
        services.AddHostedService<LogEntryCaptureWorker>();
        services.AddTransient<ILogDataItemManager, LogDataItemManager>();
        services.AddTransient<ILogEntryManager, LogEntryManager>();
        services.AddSingleton<ILoggerProvider>(
            implementationFactory: provider =>
            {
                ILogEntryCaptureQueue queue =
                    provider.GetRequiredService<ILogEntryCaptureQueue>();

                IHttpContextAccessor httpContextAccessor =
                    provider.GetRequiredService<IHttpContextAccessor>();

                LoggingConfiguration configuration =
                    provider.GetRequiredService<LoggingConfiguration>();

                return new LoggingLoggerProvider(
                    loggerFactory: categoryName =>
                        new LoggingLoggerDependency(
                            capture: (level, capturedCategoryName, message, exception) =>
                                queue.TryEnqueue(
                                    logEntryCaptureRequest: new LogEntryCaptureRequest
                                    {
                                        Level = level,
                                        CategoryName = capturedCategoryName,
                                        Message = message,
                                        Exception = exception,
                                        RequestDomain = httpContextAccessor.HttpContext?.Request.Host.Host,
                                        Persist = level >= configuration.DatabaseMinimumLogLevel,
                                    }),
                            categoryName: categoryName));
            });
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
        services.AddTransient<ILogEntryStreamService, LogEntryStreamService>();
        services.AddTransient<ILogEntryStreamProcessingService, LogEntryStreamProcessingService>();
        services.AddTransient<ILogHubBroker, LogHubBroker>();
        services.AddTransient<IAuthorizationBroker, AuthorizationBroker>();
        services.AddTransient<IMetadataBroker, MetadataBroker>();
    }

    private static void AddFoundations(this IServiceCollection services)
    {
        services.AddTransient<ILoggingMetadataTypeService, LoggingMetadataTypeService>();
        services.AddTransient<ILogDataItemService, LogDataItemService>();
        services.AddTransient<ILogEntryService, LogEntryService>();
        services.AddTransient<ILogHubService, LogHubService>();
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
            ILogEntryRetentionProcessingService,
            LogEntryRetentionProcessingService>();
    }

    private static void AddHostedServiceExposures(this IServiceCollection services)
    {
        services.AddSingleton<LogRetentionRunner>(
            implementationFactory: provider =>
            {
                IServiceScopeFactory serviceScopeFactory =
                    provider.GetRequiredService<IServiceScopeFactory>();

                return async cancellationToken =>
                {
                    using IServiceScope scope = serviceScopeFactory.CreateScope();

                    ILogEntryRetentionProcessingService logRetentionProcessingService =
                        scope.ServiceProvider.GetRequiredService<ILogEntryRetentionProcessingService>();

                    await logRetentionProcessingService.RunLogRetentionAsync(
                        cancellationToken: cancellationToken);
                };
            });
        services.AddSingleton<ILogRetentionCleaner, LogRetentionCleaner>();
        services.AddSingleton<IHostedService>(
            implementationFactory: provider =>
                provider.GetRequiredService<ILogRetentionCleaner>());
    }

    private static LoggingConfiguration CreateLoggingConfiguration(
        this IServiceCollection services) =>
        new()
        {
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