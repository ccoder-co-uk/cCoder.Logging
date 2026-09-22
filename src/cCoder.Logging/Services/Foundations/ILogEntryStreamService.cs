// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Logging.Services.Foundations;

internal interface ILogEntryStreamService
{
    ValueTask StreamLogEntryAsync(
        string thread,
        string level,
        string message);
    bool ShouldStreamLogEntries();
    int? GetDefaultAppId();
    string GetDefaultAppDomain();
}