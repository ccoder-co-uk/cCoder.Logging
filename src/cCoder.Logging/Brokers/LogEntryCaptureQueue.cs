// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Channels;
using cCoder.Logging.Models;

namespace cCoder.Logging.Brokers;

internal sealed class LogEntryCaptureQueue : ILogEntryCaptureQueue
{
    private readonly Channel<LogEntryCaptureRequest> channel;

    public LogEntryCaptureQueue(Channel<LogEntryCaptureRequest> channel) =>
        this.channel = channel;

    public bool TryEnqueue(LogEntryCaptureRequest request) =>
        channel.Writer.TryWrite(item: request);

    public IAsyncEnumerable<LogEntryCaptureRequest> ReadAllAsync() =>
        channel.Reader.ReadAllAsync();

    public void Complete() =>
        channel.Writer.TryComplete();
}