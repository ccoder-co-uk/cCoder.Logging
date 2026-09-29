// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Extensions.Logging;
using System;

namespace cCoder.Logging.Brokers.Loggings;

internal sealed class LoggingLoggerProvider(
    Func<string, ILogger> loggerFactory) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) =>
        loggerFactory.Invoke(arg: categoryName);

    public void Dispose() =>
        GC.SuppressFinalize(obj: this);
}