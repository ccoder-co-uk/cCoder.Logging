// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using cCoder.Security.Models.Configurations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Logging.Dependencies.Authentication;

internal sealed class LogHubAuthenticationDependency : IHubFilter
{
    public ValueTask<object> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object>> next)
    {
        SSOAuthInfo authentication = invocationContext.ServiceProvider
            .GetRequiredService<SSOAuthInfo>();

        string userName = invocationContext.Context.User?.Identity?.IsAuthenticated == true
            ? invocationContext.Context.User.Identity.Name
            : null;

        authentication.SSOUserId = string.IsNullOrWhiteSpace(value: userName)
            ? "Guest"
            : userName;

        return next(arg: invocationContext);
    }
}