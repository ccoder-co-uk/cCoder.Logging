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
        using HttpResponseMessage response = await Client.GetAsync("/Api/Logging/Baseline");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonDocument.Parse(content).RootElement.Clone();
    }
}