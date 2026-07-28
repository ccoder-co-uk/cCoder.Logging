// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;

namespace cCoder.Logging.Services.Processings;

internal interface ILogHubProcessingService
{
    ValueTask ConnectLogHubSessionAsync(LogHubSession session);

    ValueTask JoinLogHubSessionAsync(LogHubSession session);

    ValueTask LeaveLogHubSessionAsync(LogHubSession session);

    ValueTask DisconnectLogHubSessionAsync(LogHubSession session);

    void DebugLogHubSession(LogHubSession session);

    void InfoLogHubSession(LogHubSession session);

    void WarnLogHubSession(LogHubSession session);

    void ErrorLogHubSession(LogHubSession session);

    ValueTask SendConsoleLogHubSessionAsync(LogHubSession session);

    ValueTask SendTestLogHubSessionAsync(LogHubSession session);
}