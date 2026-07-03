using Logging.Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Logging.Web.AcceptanceTests.Tests.Api;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class HealthTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
}
