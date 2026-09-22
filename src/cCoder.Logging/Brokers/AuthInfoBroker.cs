// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.CodeAnalysis.Exposures;

namespace cCoder.Logging.Brokers;

internal interface IAuthInfoBroker
{
    string SelectCurrentSsoUserId();
}

internal sealed class AuthInfoBroker(
    ICoreAuthInfo authInfo)
        : IAuthInfoBroker, IUtilityBroker
{
    public string SelectCurrentSsoUserId() =>
        authInfo.SSOUserId;
}