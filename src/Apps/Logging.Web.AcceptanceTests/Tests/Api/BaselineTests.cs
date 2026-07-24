// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Text.Json;
using FluentAssertions;
using Logging.Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Logging.Web.AcceptanceTests.Tests.Api;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class BaselineTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;

    private async Task<JsonElement> GetBaselineAsync()
    {
        // Given
        const string baselineRoute = "/Api/Logging/Baseline";

        // When
        using HttpResponseMessage response = await Client.GetAsync(requestUri: baselineRoute);
        string content = await response.Content.ReadAsStringAsync();

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonDocument.Parse(json: content)
            .RootElement.Clone();
    }
}