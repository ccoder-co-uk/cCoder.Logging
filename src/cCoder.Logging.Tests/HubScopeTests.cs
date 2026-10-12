// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Security.Claims;
using cCoder.Data.Models.Security;
using cCoder.Logging.Brokers;
using cCoder.Logging.Dependencies.Authentication;
using cCoder.Logging.Exposures.Hubs;
using cCoder.Logging.Services.Foundations;
using cCoder.Security.Models.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Xunit.Abstractions;
using HubLog = cCoder.Logging.Brokers.Loggings.ILoggingBroker;

public sealed partial class HubScopeTests(ITestOutputHelper output)
{
    [Theory]
    [InlineData("Paul", true, 21, true)]
    [InlineData("Other", true, 21, false)]
    [InlineData("Paul", false, 21, false)]
    [InlineData("Paul", true, 22, false)]
    public async Task AuthenticatedAppAdminShouldJoinFromNewHubInvocationScope(string principalName, bool authenticated, int roleAppId, bool expectedJoin)
    {
        // Given
        const string domain = "demo.dev.corporatelinx.com";
        Guid roleId = Guid.NewGuid();
        User paul = new() { Id = "Paul", Roles = [new UserRole { UserId = "Paul", RoleId = roleId, Role = new Role { Id = roleId, AppId = roleAppId, Privs = "app_admin" } }] };
        Assert.True(condition: paul.IsAdminOfApp(appId: roleAppId));
        Mock<ILogHubBroker> broker = new();

        broker.Setup(expression: value => value.SelectAppIdByDomain(domain: domain))
            .Returns(value: 21);

        broker.Setup(expression: value => value.JoinGroupAsync(logHubSession: It.IsAny<cCoder.Logging.Models.LogHubSession>()))
            .Returns(valueFunction: (cCoder.Logging.Models.LogHubSession session) => session.Groups.AddToGroupAsync(connectionId: session.ConnectionId, groupName: session.Thread));

        broker.Setup(expression: value => value.SendCallerAsync(logHubSession: It.IsAny<cCoder.Logging.Models.LogHubSession>(), level: It.IsAny<string>(), message: It.IsAny<string>()))
            .Returns(valueFunction: (cCoder.Logging.Models.LogHubSession session, string level, string message) => session.Clients.Caller.SendCoreAsync(method: "ConsoleReceive", args: [level, message, session.Thread]));

        TaskCompletionSource joinNotification = new(creationOptions: TaskCreationOptions.RunContinuationsAsynchronously);

        string selectedIdentity = null;
        ScopeEvidence evidence = new();
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped(implementationFactory: _ => new SSOAuthInfo { SSOUserId = "Guest" });
        builder.Services.AddSingleton(implementationInstance: evidence);

        builder.Services.AddSignalR(configure: options => options.AddFilter<EvidenceFilter>())
            .AddHubOptions<LogHub>(configure: options => options.AddFilter<LogHubAuthenticationDependency>());

        builder.Services.AddSingleton(implementationInstance: broker.Object);
        builder.Services.AddSingleton(implementationInstance: Mock.Of<HubLog>());
        builder.Services.AddScoped<ILogHubService, LogHubService>();

        builder.Services.AddScoped<IAuthorizationBroker>(implementationFactory: services =>
        {
            var authorization = new Mock<IAuthorizationBroker>();

            authorization.Setup(expression: value => value.SelectCurrentUser())
                .Returns(valueFunction: () =>
            {
                selectedIdentity = services.GetRequiredService<SSOAuthInfo>().SSOUserId;
                return selectedIdentity == "Paul" ? paul : new User { Id = "Guest", Roles = [] };
            });

            return authorization.Object;
        });

        await using WebApplication app = builder.Build();

        app.Use(middleware: async (context, next) =>
        {
            // Equivalent to the deployed Security middleware after validating the token.
            context.RequestServices.GetRequiredService<SSOAuthInfo>().SSOUserId = "Paul";
            context.User = new ClaimsPrincipal(identity: new ClaimsIdentity(claims: [new Claim(type: ClaimTypes.Name, value: principalName)], authenticationType: authenticated ? "cCoder.Security" : null));
            await next(context: context);
        });

        app.MapHub<LogHub>(pattern: "/Api/Hubs/Logs");
        await app.StartAsync();

        await using HubConnection connection = new HubConnectionBuilder()
            .WithUrl(url: "http://localhost/Api/Hubs/Logs", configureHttpConnection: options =>
        {
            options.Transports = HttpTransportType.LongPolling;

            options.HttpMessageHandlerFactory = _ => app.GetTestServer()
                .CreateHandler();
        })
            .Build();

        connection.On<string, string, string>(methodName: "ConsoleReceive", handler: (_, message, thread) =>
        {
            if (thread == domain && message == $"Connected to instance {domain}")
            {
                joinNotification.TrySetResult();
            }
        });

        // When
        await connection.StartAsync();
        await connection.InvokeAsync(methodName: "Join", arg1: domain);
        output.WriteLine(message: $"Authenticated principal={evidence.Principal}; hub scoped auth={evidence.Scoped}; broker selected={selectedIdentity}; HttpContext present={evidence.HttpContextPresent}");

        if (expectedJoin)
        {
            await joinNotification.Task.WaitAsync(timeout: TimeSpan.FromSeconds(seconds: 5));
        }

        // Then
        Assert.Equal(expected: principalName, actual: evidence.Principal);
        broker.Verify(expression: value => value.JoinGroupAsync(logHubSession: It.IsAny<cCoder.Logging.Models.LogHubSession>()), times: () => Times.Exactly(callCount: expectedJoin ? 1 : 0));
    }
    public sealed class ScopeEvidence
    {
        public string Principal { get; set; }
        public string Scoped { get; set; }
        public bool HttpContextPresent { get; set; }
    }
    public sealed class EvidenceFilter(ScopeEvidence evidence) : IHubFilter
    {
        public async ValueTask<object> InvokeMethodAsync(HubInvocationContext context, Func<HubInvocationContext, ValueTask<object>> next)
        {
            evidence.Principal = context.Context.User?.Identity?.Name;
            evidence.Scoped = context.ServiceProvider.GetRequiredService<SSOAuthInfo>().SSOUserId;
            evidence.HttpContextPresent = context.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext is not null;
            return await next(arg: context);
        }
    }
}