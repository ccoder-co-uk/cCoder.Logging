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
            connection.On<string, string, string>("ConsoleReceive",
                (level, message, receivedThread) => receivedMessages.Writer.TryWrite((level, message, receivedThread)));

            // When
            await connection.InvokeAsync("Join", Thread).WaitAsync(TimeSpan.FromSeconds(10));
            _ = await receivedMessages.Reader.ReadAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(10));
            _ = await receivedMessages.Reader.ReadAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(10));
            await connection
                .InvokeAsync("ConsoleSend", "info", expectedMessage, Thread)
                .WaitAsync(TimeSpan.FromSeconds(10));
            (string level, string message, string receivedThread) actual = await receivedMessages.Reader
                .ReadAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(10));

            // Then
            actual.level.Should().Be("info");
            actual.message.Should().Be(expectedMessage);
            actual.receivedThread.Should().Be(Thread);
        }
        finally
        {
            await connection.StopAsync().WaitAsync(TimeSpan.FromSeconds(5));
            await connection.DisposeAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(5));
        }
    }
}



