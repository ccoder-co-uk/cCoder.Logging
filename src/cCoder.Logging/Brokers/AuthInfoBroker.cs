// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;

namespace cCoder.Logging.Brokers;

internal interface IAuthInfoBroker
{
    string SelectCurrentSsoUserId();
}

internal sealed class AuthInfoBroker(
    ICoreAuthInfo authInfo)
        : IAuthInfoBroker
{
    public string SelectCurrentSsoUserId() =>
        authInfo.SSOUserId;
}