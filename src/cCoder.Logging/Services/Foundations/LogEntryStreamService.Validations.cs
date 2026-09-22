// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Logging.Services.Foundations;

internal sealed partial class LogEntryStreamService
{
    private static void ValidateLogEntryOnStream(
        string thread,
        string level,
        string message)
    {
        ArgumentNullException.ThrowIfNull(argument: thread);
        ArgumentNullException.ThrowIfNull(argument: level);
        ArgumentNullException.ThrowIfNull(argument: message);
    }
}