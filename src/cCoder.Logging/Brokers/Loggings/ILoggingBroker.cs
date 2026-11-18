// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.CodeAnalysis.Exposures;

namespace cCoder.Logging.Brokers.Loggings;

public interface ILoggingBroker : IUtilityBroker
{
    void LogDebug(string message, params object[] args);
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(string message, params object[] args);
    void LogError(Exception exception, string message, params object[] args);
}