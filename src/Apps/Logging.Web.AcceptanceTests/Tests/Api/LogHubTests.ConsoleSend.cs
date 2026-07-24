// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Channels;
using Xunit;


namespace Logging.Web.AcceptanceTests.Tests.Api;

public sealed partial class LogHubTests
{
    [Fact]
    public async Task ShouldBroadcastToJoinedGroupWhenConsoleSend()
    {
        // Given
        string expectedMessage = $"acceptance-message-{Guid.NewGuid():N}";
        Channel<(string level, string message, string thread)> receivedMessages = Channel.CreateUnbounded<(string level, string message, string thread)>();
        HubConnection connection = await ConnectAsync();

        try
        {
            connection.On<string, string, string>(methodName: "ConsoleReceive",
handler: (level, message, receivedThread) => receivedMessages.Writer.TryWrite(item: (level, message, receivedThread)));

            // When

            await connection.InvokeAsync(methodName: "Join", arg1: Thread)
                .WaitAsync(timeout: TimeSpan.FromSeconds(seconds: 10));

            _ = await receivedMessages.Reader.ReadAsync()
                .AsTask()
                .WaitAsync(timeout: TimeSpan.FromSeconds(seconds: 10));

            _ = await receivedMessages.Reader.ReadAsync()
                .AsTask()
                .WaitAsync(timeout: TimeSpan.FromSeconds(seconds: 10));

            await connection
                .InvokeAsync(methodName: "ConsoleSend", arg1: "info", arg2: expectedMessage, arg3: Thread)
                .WaitAsync(timeout: TimeSpan.FromSeconds(seconds: 10));

            (string level, string message, string receivedThread) actual = await receivedMessages.Reader
                .ReadAsync()
                .AsTask()
                .WaitAsync(timeout: TimeSpan.FromSeconds(seconds: 10));

            // Then

            actual.level.Should()
                .Be(expected: "info");

            actual.message.Should()
                .Be(expected: expectedMessage);

            actual.receivedThread.Should()
                .Be(expected: Thread);
        }
        finally
        {
            await connection.StopAsync()
                .WaitAsync(timeout: TimeSpan.FromSeconds(seconds: 5));

            await connection.DisposeAsync()
                .AsTask()
                .WaitAsync(timeout: TimeSpan.FromSeconds(seconds: 5));
        }
    }
}