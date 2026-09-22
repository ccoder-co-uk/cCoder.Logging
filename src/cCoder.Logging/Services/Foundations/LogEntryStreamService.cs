// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Brokers;
using cCoder.Logging.Models;

namespace cCoder.Logging.Services.Foundations;

internal sealed partial class LogEntryStreamService(
    ILogEntryStreamBroker logEntryStreamBroker,
    LoggingConfiguration loggingConfiguration)
        : ILogEntryStreamService
{
    public ValueTask StreamLogEntryAsync(
        string thread,
        string level,
        string message) =>
        TryCatch(operation: () =>
        {
            ValidateLogEntryOnStream(
                thread: thread,
                level: level,
                message: message);

            return logEntryStreamBroker.SendLogEntryAsync(
                thread: thread,
                level: level,
                message: message);
        });

    public bool ShouldStreamLogEntries() =>
        TryCatch(operation: () => loggingConfiguration.StreamLogEntries);

    public int? GetDefaultAppId() =>
        TryCatch(operation: () => loggingConfiguration.DefaultAppId);

    public string GetDefaultAppDomain() =>
        TryCatch(operation: () => loggingConfiguration.DefaultAppDomain);
}