// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        using HttpRequestMessage request = new(method: new HttpMethod(method: method), requestUri: $"{LogEntryRoute}(1)");

        if (method is "PUT" or "PATCH")
        {
            request.Content = JsonContent.Create(inputValue: logEntry);
        }

        // When
        using HttpResponseMessage response = await Client.SendAsync(request: request);

        // Then

        response.IsSuccessStatusCode.Should()
            .BeFalse();
    }
}