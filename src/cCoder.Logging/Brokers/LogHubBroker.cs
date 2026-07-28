// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Logging.Brokers;

internal interface ILogHubBroker
{
    int? SelectAppIdByDomain(string domain);
}

internal sealed class LogHubBroker(
    ICoreContextFactory contextFactory) : ILogHubBroker
{
    public int? SelectAppIdByDomain(string domain)
    {
        using CoreDataContext context =
            contextFactory.CreateCoreContext();

        return context.Apps
            .IgnoreQueryFilters()
            .Where(predicate: app => app.Domain == domain)
            .Select(selector: app => (int?)app.Id)
            .FirstOrDefault();
    }
}