// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using cCoder.Data.Exposures;
using cCoder.Data.Models.Security;
using cCoder.Logging.Brokers;
using cCoder.Logging.Exposures.HostedServices;
using cCoder.Logging.Models;
using cCoder.Logging.Services.Processings;
using cCoder.Security.Models.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace cCoder.Logging.Tests;

public sealed partial class LogStreamIntegrationTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ShouldDeliverLogsInOrderAfterAnAppAdminJoinsAsync(bool hostedServices)
    {
        // Given
        const string domain = "web.example.test";
        const int appId = 21;
        const string testUser = "log-admin";
        Guid roleId = Guid.NewGuid();

        User admin = new()
        {
            Id = testUser,
            Roles =
            [
                new UserRole
                {
                    UserId = testUser,
                    RoleId = roleId,
                    Role = new Role { Id = roleId, AppId = appId, Privs = "app_admin" }
                }
            ]
        };

        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();

        LoggingConfiguration configuration = new()
        {
            StreamLogEntries = true,
            RequestLoggingQueueCapacity = 256,
            DefaultAppDomain = hostedServices ? domain : null,
            DatabaseMinimumLogLevel = LogLevel.Warning
        };

        if (hostedServices)
        {
            builder.Services.AddLoggingHostedServices(configuration: configuration);
            builder.Services.AddSingleton(implementationInstance: Mock.Of<ILogRetentionCleaner>());
        }
        else
        {
            builder.Services.AddLoggingWeb(configuration: configuration);
        }

        builder.Services.AddScoped(implementationFactory: _ => new SSOAuthInfo { SSOUserId = "Guest" });

        builder.Services.AddScoped(implementationFactory: provider =>
        {
            Mock<IAuthorizationBroker> broker = new();

            broker.Setup(expression: authorization => authorization.SelectCurrentUser())
                .Returns(valueFunction: () =>
                    provider.GetRequiredService<SSOAuthInfo>().SSOUserId == testUser
                        ? admin
                        : new User { Id = "Guest", Roles = [] });

            return broker.Object;
        });

        Mock<IMetadataTypeCache> metadataCache = new();

        metadataCache.Setup(expression: cache => cache.Contains(scope: "Logging"))
            .Returns(value: true);

        builder.Services.AddSingleton(implementationInstance: metadataCache.Object);

        Mock<ILogHubBroker> hubBroker = new();

        hubBroker.Setup(expression: broker => broker.SelectAppIdByDomain(domain: domain))
            .Returns(value: appId);

        hubBroker.Setup(expression: broker =>
                broker.JoinGroupAsync(logHubSession: It.IsAny<LogHubSession>()))
            .Returns(valueFunction: (LogHubSession session) =>
                session.Groups.AddToGroupAsync(connectionId: session.ConnectionId, groupName: session.Thread));

        hubBroker.Setup(expression: broker => broker.SendCallerAsync(
                logHubSession: It.IsAny<LogHubSession>(),
                level: It.IsAny<string>(),
                message: It.IsAny<string>()))
            .Returns(valueFunction: (LogHubSession session, string level, string message) =>
                session.Clients.Caller.SendAsync(method: "ConsoleReceive", arg1: level, arg2: message, arg3: session.Thread));

        builder.Services.AddSingleton(implementationInstance: hubBroker.Object);

        Mock<ILogEntryCaptureProcessingService> persistence = new();

        persistence.Setup(expression: service => service.CaptureLogEntryCaptureOperationAsync(
                operation: It.IsAny<LogEntryCaptureOperation>()))
            .Returns(valueFunction: (LogEntryCaptureOperation operation) => ValueTask.FromResult(result: operation));

        builder.Services.AddSingleton(implementationInstance: persistence.Object);
        builder.Services.AddSingleton(implementationInstance: Mock.Of<ILogEntryEventProcessingService>());

        await using WebApplication app = builder.Build();

        app.Use(middleware: async (context, next) =>
        {
            context.User = new ClaimsPrincipal(
                identity: new ClaimsIdentity(
                    claims: [new Claim(type: ClaimTypes.Name, value: testUser)],
                    authenticationType: "Test"));

            await next(context: context);
        });

        app.StartLoggingWeb();

        string prefix = $"live-{Guid.NewGuid():N}";
        string[] messages = [$"{prefix}-1", $"{prefix}-2", $"{prefix}-3"];

        ILogger appLogger = app.Services.GetRequiredService<ILoggerFactory>()
            .CreateLogger(categoryName: "Application");

        app.MapGet(pattern: "/activity", handler: () =>
        {
            foreach (string message in messages)
            {
                appLogger.LogInformation(message: "{Activity}", args: message);
            }
        });

        using CancellationTokenSource timeout = new(delay: TimeSpan.FromSeconds(seconds: 10));
        await app.StartAsync(cancellationToken: timeout.Token);

        await using HubConnection connection = new HubConnectionBuilder()
            .WithUrl(url: $"http://{domain}/Api/Hubs/Logs", configureHttpConnection: options =>
            {
                options.Transports = HttpTransportType.LongPolling;

                options.HttpMessageHandlerFactory = _ => app.GetTestServer()
                    .CreateHandler();
            })
            .Build();

        Channel<(string Level, string Message, string Domain)> received =
            Channel.CreateUnbounded<(string, string, string)>();

        using IDisposable subscription = connection.On<string, string, string>(
            methodName: "ConsoleReceive", handler: (level, message, thread) =>
            {
                if (message.StartsWith(value: prefix, comparisonType: StringComparison.Ordinal)
                    || message == $"Connected to instance {domain}")
                {
                    _ = received.Writer.TryWrite(item: (level, message, thread));
                }
            });

        await connection.StartAsync(cancellationToken: timeout.Token);
        await connection.InvokeAsync(methodName: "Join", arg1: domain, cancellationToken: timeout.Token);

        Assert.Equal(
            expected: ("info", $"Connected to instance {domain}", domain),
            actual: await received.Reader.ReadAsync(cancellationToken: timeout.Token));

        // When
        if (hostedServices)
        {
            foreach (string message in messages)
            {
                appLogger.LogInformation(message: "{Activity}", args: message);
            }
        }
        else
        {
            using System.Net.Http.HttpClient client = app.GetTestClient();

            using var response = await client.GetAsync(
                requestUri: $"http://{domain}/activity", cancellationToken: timeout.Token);

            response.EnsureSuccessStatusCode();
        }

        // Then
        foreach (string message in messages)
        {
            Assert.Equal(
                expected: ("information", message, domain),
                actual: await received.Reader.ReadAsync(cancellationToken: timeout.Token));
        }
    }
}