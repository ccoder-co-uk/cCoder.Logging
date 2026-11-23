// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Logging.Models;

public sealed class LogStreamMessage
{
    public const string EventName = "log_stream";
    public string Domain { get; init; }
    public string Level { get; init; }
    public string Message { get; init; }
}