// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using Logging.Web.AcceptanceTests.Infrastructure;
using System.Net.Http.Json;
using Xunit;

namespace Logging.Web.AcceptanceTests.Tests.Api;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class AuthenticationTests(WebAcceptanceFixture fixture)
{
    private const string Password = "TestPass01!";

    private HttpClient Client { get; } = fixture.Client;

    private static RegisterUser CreateRegisterUser() =>
        new()
        {
            DisplayName = "Logging Acceptance User",
            Email = $"logging.acceptance.{Guid.NewGuid():N}@example.com",
            Password = Password,
            Culture = "en-GB",
            PhoneNumber = "01234567890",
        };

    private async ValueTask<RegisterUser> RegisterUserAsync()
    {
        RegisterUser user = CreateRegisterUser();

        using HttpResponseMessage response = await Client.PostAsJsonAsync(
            "/Api/Account/Register",
            user);
        await EnsureSuccessAsync(response);

        return user;
    }

    private async ValueTask<Token> LoginAsync(RegisterUser user)
    {
        Auth auth = new()
        {
            User = user.Email,
            Pass = user.Password,
        };

        using HttpResponseMessage response = await Client.PostAsJsonAsync(
            "/Api/Account/Login",
            auth);
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<Token>();
    }

    private static async ValueTask EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        string content = await response.Content.ReadAsStringAsync();

        throw new InvalidOperationException(
            $"Expected success but received {(int)response.StatusCode} {response.ReasonPhrase}: {content}");
    }
}