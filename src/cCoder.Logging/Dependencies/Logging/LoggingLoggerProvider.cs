// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Dependencies.Logging;
using cCoder.Logging.Services.Orchestrations;

namespace cCoder.Logging.Dependencies.Logging;

internal sealed class LoggingLoggerProvider(IServiceProvider serviceProvider) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) =>
        new LoggingLogger(serviceProvider, categoryName);

    public void Dispose() => GC.SuppressFinalize(this);
}

internal sealed class LoggingLogger(
    IServiceProvider serviceProvider,
    string categoryName) : ILogger
{
    private static readonly AsyncLocal<bool> IsCapturing = new();

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
        if (formatter is null || IsCapturing.Value)
            return;

        string message = formatter(state, exception);

        if (string.IsNullOrWhiteSpace(message))
            return;

        _ = CaptureAsync(logLevel, message, exception);
    }

    private async Task CaptureAsync(LogLevel logLevel, string message, Exception exception)
    {
        try
        {
            IsCapturing.Value = true;

            using IServiceScope scope = serviceProvider.CreateScope();
            IHttpContextAccessor httpContextAccessor = scope.ServiceProvider.GetService<IHttpContextAccessor>();
            ILogEntryCaptureOrchestrationService captureService =
                scope.ServiceProvider.GetRequiredService<ILogEntryCaptureOrchestrationService>();

            LogEntryCaptureRequest logEntryCaptureRequest = new()
            {
                Level = logLevel,
                CategoryName = categoryName,
                Message = message,
                Exception = exception,
                RequestDomain = httpContextAccessor?.HttpContext?.Request?.Host.Host
            };

            await captureService.CaptureLogEntryAsync(
                logEntryCaptureRequest: logEntryCaptureRequest);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            IsCapturing.Value = false;
        }
    }
}