// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net.Http;

using Logging.Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Logging.Web.AcceptanceTests.Tests.Api;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class LogDataItemControllerTests(
    WebAcceptanceFixture fixture)
{
    private const string LogDataItemRoute = "/Api/Logging/LogDataItem";
    private HttpClient Client { get; } = fixture.Client;
}