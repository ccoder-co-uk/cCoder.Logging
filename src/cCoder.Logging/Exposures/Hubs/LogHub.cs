// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using IAuthorizationBroker = cCoder.Logging.Brokers.IAuthorizationBroker;


namespace cCoder.Logging.Exposures.Hubs;

public class LogHub : Hub
{
    private readonly ILogger log;
    private readonly CoreDataContext coreDataContext;
    private readonly IAuthorizationBroker authorizationBroker;
    private static readonly IDictionary<string, ICollection<HistoryItem>> History =
        new Dictionary<string, ICollection<HistoryItem>>();
    private static readonly IDictionary<string, int> UserCounts = new Dictionary<string, int>();

    public LogHub(
        CoreDataContext coreDataContext,
        IAuthorizationBroker authorizationBroker,
        ILogger<LogHub> log
    )
    {
        this.coreDataContext = coreDataContext;
        this.authorizationBroker = authorizationBroker;
        this.log = log;
    }

    public struct HistoryItem
    {
        public string Level { get; set; }
        public string Message { get; set; }
    }

    public override Task OnConnectedAsync()
    {
        log.LogDebug($"New Client connected to {GetType().Name}");
        return base.OnConnectedAsync();
    }

    public async Task Join(string thread)
    {
        log.LogDebug($"User joining {thread}");

        int? app = coreDataContext.Apps
            .IgnoreQueryFilters()
            .Where(app => app.Domain == thread)
            .Select(app => (int?)app.Id)
            .FirstOrDefault();
        User user = authorizationBroker.GetCurrentUser();

        if (app.HasValue && user.IsAdminOfApp(app.Value))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, thread);
            await Clients.Caller.SendAsync(
                "ConsoleReceive",
                "info",
                "Connected to instance " + thread,
                thread
            );
            await Clients.Group(thread).SendAsync("ConsoleReceive", "info", "User Joined", thread);
            log.LogInformation($"User {user.Id} is listening to log stream for domain {thread}");

            if (!History.ContainsKey(thread))
                History.Add(thread, new List<HistoryItem>());

            if (!UserCounts.ContainsKey(thread))
                UserCounts.Add(thread, 1);
            else
                UserCounts[thread]++;

            foreach (HistoryItem item in History[thread])
                await Clients.Caller.SendAsync("ConsoleReceive", item.Level, item.Message, thread);
        }
        else
        {
            log.LogWarning($"User {user.Id} denied access to log stream for domain {thread}");
        }
    }

    public async Task Leave(string thread)
    {
        log.LogDebug($"User leaving {thread}");

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, thread);
        await Clients.Caller.SendAsync(
            "info",
            "Stopped listening to messages for " + thread,
            thread
        );
        await Clients.Group(thread).SendAsync("ConsoleReceive", "info", "User Left", thread);
        UserCounts[thread]--;

        if (UserCounts[thread] == 0)
            History.Remove(thread);

        User user = authorizationBroker.GetCurrentUser();
        log.LogInformation($"User {user.Id} stopped listening to log stream for domain {thread}");
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        User user = authorizationBroker.GetCurrentUser();
        log.LogInformation($"User {user.Id} disconnected.");
        return Task.CompletedTask;
    }

    public void Debug(string level, string message) =>
        log.LogDebug($"{Context.GetHttpContext().Request.Host.Value}: {level} {message}");

    public void Info(string level, string message) =>
        log.LogInformation($"{Context.GetHttpContext().Request.Host.Value}: {level} {message}");

    public void Warn(string level, string message) =>
        log.LogWarning($"{Context.GetHttpContext().Request.Host.Value}: {level} {message}");

    public void Error(string level, string message) =>
        log.LogError($"{Context.GetHttpContext().Request.Host.Value}: {level} {message}");

    public async Task ConsoleSend(string level, string message, string thread)
    {
        if (!History.ContainsKey(thread))
            History.Add(thread, new List<HistoryItem>());

        History[thread].Add(new HistoryItem { Message = message, Level = level });
        await Clients.Group(thread).SendAsync("ConsoleReceive", level, message, thread);
    }

    public virtual async Task SendTest(string message, string thread) =>
        await Clients.Group(thread).SendAsync("ConsoleReceive", "test", message, thread);
}