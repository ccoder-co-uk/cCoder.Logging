// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;

using cCoder.Logging.Brokers;
using cCoder.Logging.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace cCoder.Logging.Tests;

public sealed partial class LogEntryCaptureQueueTests
{
    [Theory]
    [InlineData(RequestLoggingQueueFullBehavior.DropNewest, "drop_newest", "first", "second")]
    [InlineData(RequestLoggingQueueFullBehavior.DropOldest, "drop_oldest", "second", "third")]
    public async Task ShouldRecordDropsAndKeepRemainingEntriesInOrderAsync(
        RequestLoggingQueueFullBehavior behavior,
        string dropReason,
        string firstRetained,
        string secondRetained)
    {
        // Given
        ServiceCollection services = new();

        services.AddLoggingWeb(configuration: new LoggingConfiguration
        {
            RequestLoggingQueueCapacity = 2,
            RequestLoggingQueueFullBehavior = behavior
        });

        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        ILogEntryCaptureQueue queue = serviceProvider.GetRequiredService<ILogEntryCaptureQueue>();
        long drops = 0;
        int writerThread = Environment.CurrentManagedThreadId;
        using MeterListener listener = new();

        listener.InstrumentPublished = (instrument, currentListener) =>
        {
            if (instrument.Meter.Name == "cCoder.Logging"
                && instrument.Name == "ccoder.logging.capture.dropped")
            {
                currentListener.EnableMeasurementEvents(instrument: instrument);
            }
        };

        listener.SetMeasurementEventCallback<long>(measurementCallback: (_, count, tags, _) =>
        {
            if (Environment.CurrentManagedThreadId == writerThread
                && tags.Length == 1
                && tags[0].Key == "reason"
                && Equals(objA: tags[0].Value, objB: dropReason))
            {
                drops += count;
            }
        });

        listener.Start();

        // When
        Assert.True(condition: queue.TryEnqueue(logEntryCaptureRequest: new LogEntryCaptureRequest { Message = "first" }));
        Assert.True(condition: queue.TryEnqueue(logEntryCaptureRequest: new LogEntryCaptureRequest { Message = "second" }));
        Assert.True(condition: queue.TryEnqueue(logEntryCaptureRequest: new LogEntryCaptureRequest { Message = "third" }));
        queue.Complete();

        List<string> retained = [];

        await foreach (LogEntryCaptureRequest request in queue.ReadAllAsync())
        {
            retained.Add(item: request.Message);
        }

        // Then
        Assert.Equal(expected: new[] { firstRetained, secondRetained }, actual: retained);
        Assert.Equal(expected: 1, actual: drops);
    }
}