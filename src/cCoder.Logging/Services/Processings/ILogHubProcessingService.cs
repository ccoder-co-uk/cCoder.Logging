// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;

namespace cCoder.Logging.Services.Processings;

internal interface ILogHubProcessingService
{
    ValueTask ConnectLogHubSessionAsync(LogHubSession logHubSession);

    ValueTask JoinLogHubSessionAsync(LogHubSession logHubSession);

    ValueTask LeaveLogHubSessionAsync(LogHubSession logHubSession);

    ValueTask DisconnectLogHubSessionAsync(LogHubSession logHubSession);

    void DebugLogHubSession(LogHubSession logHubSession);

    void InfoLogHubSession(LogHubSession logHubSession);

    void WarnLogHubSession(LogHubSession logHubSession);

    void ErrorLogHubSession(LogHubSession logHubSession);

    ValueTask SendConsoleLogHubSessionAsync(LogHubSession logHubSession);

    ValueTask SendTestLogHubSessionAsync(LogHubSession logHubSession);
}