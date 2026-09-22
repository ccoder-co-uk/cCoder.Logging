// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Security;
using cCoder.CodeAnalysis.Exposures;


namespace cCoder.Logging.Brokers;

internal interface IAuthorizationBroker
{
    User SelectCurrentUser();
}

internal sealed class AuthorizationBroker(
    ICoreContextFactory coreContextFactory)
        : IAuthorizationBroker, IUtilityBroker
{
    public User SelectCurrentUser()
    {
        using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return coreDataContext.User;
    }

}