// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Logging;
using cCoder.Logging.Brokers;
using cCoder.Logging.Models;
using cCoder.Logging.Exposures.Hubs;
using cCoder.Logging.Services.Foundations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace cCoder.Logging.Services.Processings;

internal sealed partial class LogEntryCaptureProcessingService(
    ILogEntryService logEntryService,
    ILogEntryStreamBroker logEntryStreamBroker,
    LoggingConfiguration loggingConfiguration)
        : ILogEntryCaptureProcessingService
{
    public ValueTask<LogEntryCaptureOperation>
        CaptureLogEntryCaptureOperationAsync(
            LogEntryCaptureOperation operation) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [operation]);

            LogEntryCaptureRequest logEntryCaptureRequest =
                operation.Request;

            if (ShouldIgnore(logEntryCaptureRequest: logEntryCaptureRequest))
            {
                return operation;
            }

            string thread = GetThread(
                logEntryCaptureRequest: logEntryCaptureRequest);

            await StreamLogEntryAsync(
                logEntryCaptureRequest: logEntryCaptureRequest,
                thread: thread);

            if (!loggingConfiguration.StoreLogEntries)
            {
                return operation;
            }

            int? appId = ResolveAppId(
                logEntryCaptureRequest: logEntryCaptureRequest,
                thread: thread);

            if (!appId.HasValue)
            {
                return operation;
            }

            LogEntry newLogEntry = CreateLogEntry(
                logEntryCaptureRequest: logEntryCaptureRequest,
                thread: thread,
                appId: appId.Value);

            operation.Result =
                await logEntryService.AddSystemLogEntryAsync(
                newLogEntry: newLogEntry);

            return operation;
        });

    private async ValueTask StreamLogEntryAsync(
        LogEntryCaptureRequest logEntryCaptureRequest,
        string thread)
    {
        if (loggingConfiguration.StreamLogEntries
            && !string.IsNullOrWhiteSpace(value: thread))
        {
            IHubContext<LogHub> hubContext =
                logEntryStreamBroker.SelectLogHubContext();

            if (hubContext is null)
            {
                return;
            }

            string level = logEntryCaptureRequest.Level
                .ToString()
                .ToLowerInvariant();

            await logEntryStreamBroker.SendLogEntryAsync(
                hubContext: hubContext,
                thread: thread,
                level: level,
                message: logEntryCaptureRequest.Message);
        }
    }

    private int? ResolveAppId(
        LogEntryCaptureRequest logEntryCaptureRequest,
        string thread)
    {
        if (loggingConfiguration.DefaultAppId.GetValueOrDefault() > 0)
        {
            return loggingConfiguration.DefaultAppId;
        }

        return logEntryService.ResolveAppId(domainOrName: thread)
            ?? logEntryService.ResolveAppId(
                domainOrName: logEntryCaptureRequest.RequestDomain)
            ?? logEntryService.ResolveAppId(
                domainOrName: loggingConfiguration.DefaultAppDomain);
    }

    private string GetThread(
        LogEntryCaptureRequest logEntryCaptureRequest) =>
        FirstValue(
            values:
            [
                logEntryCaptureRequest.RequestDomain,
                loggingConfiguration.DefaultAppDomain,
                loggingConfiguration.DefaultAppId?.ToString()
            ]);

    private static LogEntry CreateLogEntry(
        LogEntryCaptureRequest logEntryCaptureRequest,
        string thread,
        int appId)
    {
        string message = logEntryCaptureRequest.Exception is null
            ? logEntryCaptureRequest.Message
            : $"{logEntryCaptureRequest.Message}{Environment.NewLine}" +
                $"{logEntryCaptureRequest.Exception}";

        return new LogEntry
        {
            AppId = appId,
            AppName = thread,
            TypeName = logEntryCaptureRequest.CategoryName,
            Message = message,
            Level = ToLoggingLevel(logLevel: logEntryCaptureRequest.Level),
            Date = DateTime.UtcNow,
            Data = []
        };
    }

    private static string FirstValue(params string[] values) =>
        values.FirstOrDefault(
            predicate: value => !string.IsNullOrWhiteSpace(value: value));

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

    private static bool ShouldIgnore(
        LogEntryCaptureRequest logEntryCaptureRequest) =>
        logEntryCaptureRequest.Level == LogLevel.None
        || string.IsNullOrWhiteSpace(
            value: logEntryCaptureRequest.Message)
        || ShouldIgnoreCategory(
            categoryName: logEntryCaptureRequest.CategoryName);

    private static bool ShouldIgnoreCategory(string categoryName) =>
        string.IsNullOrWhiteSpace(value: categoryName)
        || categoryName.StartsWith(
            value: "Microsoft.AspNetCore.SignalR",
            comparisonType: StringComparison.Ordinal)
        || categoryName.StartsWith(
            value: "Microsoft.AspNetCore.Http.Connections",
            comparisonType: StringComparison.Ordinal)
        || categoryName.StartsWith(
            value: "System.Net.Http.HttpClient",
            comparisonType: StringComparison.Ordinal)
        || categoryName.StartsWith(
            value: "Microsoft.EntityFrameworkCore",
            comparisonType: StringComparison.Ordinal)
        || categoryName.StartsWith(
            value: "cCoder.Logging.",
            comparisonType: StringComparison.Ordinal)
        || categoryName == typeof(LogHub).FullName;
}