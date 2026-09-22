// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Logging.Dependencies.Logging;

internal sealed class LoggingLoggerDependency(
    Action<LogLevel, string, string, Exception> capture,
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
            capture(
                arg1: logLevel,
                arg2: categoryName,
                arg3: message,
                arg4: exception);
        }
    }
}