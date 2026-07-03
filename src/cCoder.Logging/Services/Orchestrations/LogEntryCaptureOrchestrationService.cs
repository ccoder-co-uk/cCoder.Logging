using cCoder.Data.Models.Logging;
using cCoder.Logging.Brokers;
using cCoder.Logging.Exposures.Hubs;
using cCoder.Logging.Models;
using cCoder.Logging.Services.Processings;
using Microsoft.Extensions.Logging;

namespace cCoder.Logging.Services.Orchestrations;

internal class LogEntryCaptureOrchestrationService(
    ILogEntryProcessingService logEntryProcessingService,
    ILogEntryEventProcessingService logEntryEventProcessingService,
    ILogEntryStreamBroker logEntryStreamBroker,
    LoggingConfiguration configuration) : ILogEntryCaptureOrchestrationService
{
    public async ValueTask CaptureAsync(LogEntryCaptureRequest request)
    {
        if (request is null
            || request.Level == LogLevel.None
            || string.IsNullOrWhiteSpace(request.Message)
            || ShouldIgnoreCategory(request.CategoryName))
        {
            return;
        }

        string thread = GetThread(request);
        string level = request.Level.ToString().ToLowerInvariant();

        if (configuration.StreamLogEntries)
            await logEntryStreamBroker.StreamAsync(thread, level, request.Message);

        if (!configuration.StoreLogEntries)
            return;

        int? appId = ResolveAppId(request, thread);

        if (!appId.HasValue)
            return;

        LogEntry logEntry = new()
        {
            AppId = appId.Value,
            AppName = thread,
            TypeName = request.CategoryName,
            Message = request.Exception is null
                ? request.Message
                : $"{request.Message}{Environment.NewLine}{request.Exception}",
            Level = ToLoggingLevel(request.Level),
            Date = DateTime.UtcNow,
            Data = Array.Empty<LogDataItem>()
        };

        LogEntry result = await logEntryProcessingService.AddSystemAsync(logEntry);
        await logEntryEventProcessingService.RaiseLogEntryAddEventAsync(result);
    }

    private int? ResolveAppId(LogEntryCaptureRequest request, string thread)
    {
        if (configuration.DefaultAppId.GetValueOrDefault() > 0)
            return configuration.DefaultAppId;

        return logEntryProcessingService.ResolveAppId(thread)
            ?? logEntryProcessingService.ResolveAppId(request.RequestDomain)
            ?? logEntryProcessingService.ResolveAppId(configuration.DefaultAppDomain);
    }

    private string GetThread(LogEntryCaptureRequest request) =>
        FirstValue(request.RequestDomain, configuration.DefaultAppDomain, configuration.DefaultAppId?.ToString());

    private static string FirstValue(params string[] values) =>
        values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));

    private static int ToLoggingLevel(LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.Critical => (int)Models.LoggingLevel.Error,
            LogLevel.Error => (int)Models.LoggingLevel.Error,
            LogLevel.Warning => (int)Models.LoggingLevel.Warning,
            LogLevel.Debug => (int)Models.LoggingLevel.Debug,
            LogLevel.Trace => (int)Models.LoggingLevel.Debug,
            _ => (int)Models.LoggingLevel.Info
        };

    private static bool ShouldIgnoreCategory(string categoryName) =>
        string.IsNullOrWhiteSpace(categoryName)
        || categoryName.StartsWith("Microsoft.AspNetCore.SignalR", StringComparison.Ordinal)
        || categoryName.StartsWith("Microsoft.AspNetCore.Http.Connections", StringComparison.Ordinal)
        || categoryName.StartsWith("System.Net.Http.HttpClient", StringComparison.Ordinal)
        || categoryName.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal)
        || categoryName.StartsWith("cCoder.Logging.", StringComparison.Ordinal)
        || categoryName == typeof(LogHub).FullName;
}
