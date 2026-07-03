using cCoder.Data.Models.Logging;
using FluentAssertions;
using System.Net.Http.Json;
using Xunit;

namespace Logging.Web.AcceptanceTests.Tests.Api;

public sealed partial class LogEntryControllerTests
{
    [Theory]
    [InlineData("PUT")]
    [InlineData("PATCH")]
    [InlineData("DELETE")]
    public async Task ShouldRejectUnsupportedWriteVerb(string method)
    {
        // Given
        LogEntry logEntry = CreateLogEntry();
        using HttpRequestMessage request = new(new HttpMethod(method), $"{LogEntryRoute}(1)");

        if (method is "PUT" or "PATCH")
            request.Content = JsonContent.Create(logEntry);

        // When
        using HttpResponseMessage response = await Client.SendAsync(request);

        // Then
        response.IsSuccessStatusCode.Should().BeFalse();
    }
}
