// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Brokers;
using cCoder.Logging.Models;

namespace cCoder.Logging.Dependencies.Logging;

internal sealed class LoggingLoggerProvider(
    ILogEntryCaptureQueue queue,
    LoggingConfiguration configuration) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) =>
        new LoggingLogger(
            queue: queue,
            configuration: configuration,
            categoryName: categoryName);

    public void Dispose() => GC.SuppressFinalize(this);
}

internal sealed class LoggingLogger(
    ILogEntryCaptureQueue queue,
    LoggingConfiguration configuration,
    string categoryName) : ILogger
{
    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        if (formatter is null
            || categoryName.StartsWith(
                value: "cCoder.Logging.",
                comparisonType: StringComparison.Ordinal))
        {
            return;
        }

        string message = formatter(arg1: state, arg2: exception);

        if (!string.IsNullOrWhiteSpace(value: message))
        {
            queue.TryEnqueue(
                request: new LogEntryCaptureRequest
                {
                    Level = logLevel,
                    CategoryName = categoryName,
                    Message = message,
                    Exception = exception,
                    Persist = logLevel >= configuration.DatabaseMinimumLogLevel
                });
        }
    }
}