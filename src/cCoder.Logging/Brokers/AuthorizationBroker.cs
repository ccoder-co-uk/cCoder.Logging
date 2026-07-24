// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;


namespace cCoder.Logging.Brokers;

internal interface IAuthorizationBroker
{
    User SelectCurrentUser();
    User SelectUserById(string userId);
    App SelectAppById(int appId);
}

internal sealed class AuthorizationBroker(
    ICoreContextFactory coreContextFactory)
        : IAuthorizationBroker
{
    public User SelectCurrentUser()
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return coreDataContext.User;
    }

    public User SelectUserById(string userId)
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return coreDataContext.Users
            .Include(navigationPropertyPath: foundUser => foundUser.Roles)
            .FirstOrDefault(predicate: foundUser => foundUser.Id == userId);
    }

    public App SelectAppById(int appId)
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return coreDataContext.Apps
            .Include(navigationPropertyPath: foundApp =>
                foundApp.Roles.Select(selector: role => role.Users))
            .FirstOrDefault(predicate: foundApp => foundApp.Id == appId);
    }
}
