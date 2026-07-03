using Logging.HostedServices.AcceptanceTests.Infrastructure;
using Xunit;

namespace Logging.HostedServices.AcceptanceTests.Tests.Api;

[Collection(HostedServicesAcceptanceCollection.Name)]
public sealed partial class HealthTests(HostedServicesAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
}
