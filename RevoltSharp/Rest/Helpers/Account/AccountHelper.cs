using Newtonsoft.Json.Linq;
using RevoltSharp.Rest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RevoltSharp;

public static class AccountHelper
{
    public static async Task<OnboardStatus> GetAccountOnboardingStatus(this RevoltRestClient rest)
    {
        OnboardStatus Json = await rest.GetAsync<OnboardStatus>("onboard/hello");
        return Json;
    }

    public static async Task CompleteAccountOnboardingAsync(this RevoltRestClient rest, string username)
    {
        await rest.PostAsync<OnboardStatus>("onboard/complete", new AccountOnboardingRequest
        {
            username = username
        });
    }

    public static async Task<AccountSession[]?> GetAccountSessionsAsync(this RevoltRestClient rest)
    {
        AccountSessionJson[] Json = await rest.GetAsync<AccountSessionJson[]>("auth/session/all");
        if (Json == null)
            return null;

        return Json.Select(x => new AccountSession(x, rest.Client)).ToArray();
    }

    public static async Task DeleteAllAccountSessionsAsync(this RevoltRestClient rest, bool includeCurrentSession)
    {
        await rest.DeleteAsync("auth/session/all?revoke_self=" + includeCurrentSession);
    }

    public static Task DeleteAsync(this AccountSession session)
        => DeleteAccountSessionAsync(session.Client.Rest, session.Id);

    public static Task DeleteAccountSessionAsync(this RevoltRestClient rest, AccountSession session)
        => DeleteAccountSessionAsync(rest, session.Id);

    public static async Task DeleteAccountSessionAsync(this RevoltRestClient rest, string sessionId)
    {
        await rest.DeleteAsync("/auth/session/" + sessionId);
    }

    public static Task ModifyAsync(this AccountSession session, string sessionName)
        => ModifyAccountSessionAsync(session.Client.Rest, session.Id, sessionName);

    public static Task ModifyAccountSessionAsync(this RevoltRestClient rest, AccountSession session, string sessionName)
        => ModifyAccountSessionAsync(rest, session.Id, sessionName);

    public static async Task ModifyAccountSessionAsync(this RevoltRestClient rest, string sessionId, string sessionName)
    {
        await rest.PatchAsync<dynamic>("auth/session/" + sessionId, new ModifyAccountSessionRequest
        {
            friendly_name = sessionName
        });
    }

    public static async Task<Dictionary<string, Tuple<long, string>>> GetAccountSettingsAsync(this RevoltRestClient rest, string[] keys)
    {
        Dictionary<string, JArray> Values = await rest.PostAsync<Dictionary<string, JArray>>("sync/settings/fetch", new AccountFetchSettingsRequest
        {
            keys = keys
        });

        return Values.ToDictionary(x => x.Key, x => new Tuple<long, string>(x.Value.First().Value<long>(), x.Value.Last().Value<string>()));
    }

    public static async Task ModifyAccountSettingsAsync(this RevoltRestClient rest, Dictionary<string, string> settings)
    {
        await rest.SendRequestAsync<dynamic>(RequestType.Post, "sync/settings/set?timestamp=" + DateTime.UtcNow.ToTimestamp(), settings);
    }

    public static async Task<ChannelReadState[]?> GetAccountUnreadsAsync(this RevoltRestClient rest)
    {
        ChannelReadStateJson[] Json = await rest.GetAsync<ChannelReadStateJson[]>("sync/unreads");
        if (Json == null)
            return null;

        return Json.Select(x => new ChannelReadState(x)).ToArray();
    }

    public static async Task CreateAccountAsync(this RevoltRestClient rest, string email, string password, string? inviteCode = null, string? captchaCode = null)
    {
        await rest.PostAsync("auth/account/create", new CreateAccountRequest
        {
            email = email,
            password = password,
            invite = inviteCode,
            captcha = captchaCode
        });
    }

    public static async Task VerifyAccountAsync(this RevoltRestClient rest, string email, string? captchaCode)
    {
        await rest.PostAsync("auth/account/reverify", new AccountVerificationRequest
        {
            email = email,
            captcha = captchaCode
        });
    }

    public static async Task RequestAccountDeletionAsync(this RevoltRestClient rest)
    {
        await rest.PostAsync("auth/account/delete");
    }

    public static async Task ConfirmAccountDeletionAsync(this RevoltRestClient rest, string token)
    {
        await rest.PutAsync("auth/account/delete", new AccountConfirmDeletionRequest
        {
            token = token
        });
    }

    public static async Task<AccountInfo?> GetAccountInfoAsync(this RevoltRestClient rest)
    {
        AccountInfoJson? json = await rest.GetAsync<AccountInfoJson>("auth/account");
        if (json == null)
            return null;

        return new AccountInfo(rest.Client, json);
    }

    public static async Task DisableAccountAsync(this RevoltRestClient rest)
    {
        await rest.PostAsync("auth/account/disable"); 
    }

    public static async Task ChangeAccountPasswordAsync(this RevoltRestClient rest, string currentPassword, string newPassword)
    {
        await rest.PatchAsync("auth/account/change/password", new AccountChangePasswordRequest
        {
            current_password = currentPassword,
            password = newPassword
        });
    }

    public static async Task ChangeAccountEmailAsync(this RevoltRestClient rest, string currentPassword, string newEmail)
    {
        await rest.PatchAsync("auth/account/change/email", new AccountChangeEmailRequest
        {
            current_password = currentPassword,
            email = newEmail
        });
    }

    public static async Task RequestPasswordResetAsync(this RevoltRestClient rest, string email, string? captchaCode = null)
    {
        await rest.PostAsync("auth/account/reset_password", new PasswordResetRequest
        {
            email = email,
            captcha = captchaCode
        });
    }

    public static async Task ConfirmPasswordResetAsync(this RevoltRestClient rest, string token, string password, bool removeSessions = false)
    {
        await rest.PatchAsync("auth/account/reset_password", new AccountPasswordResetRequest
        {
            token = token,
            password = password,
            remove_sessions = removeSessions
        });
    }

    public static async Task AccountLogoutAsync(this RevoltRestClient rest)
    {
        await rest.PostAsync("auth/session/logout");
    }

    public static async Task<AccountMFA?> AccountMFAInfoAsync(this RevoltRestClient rest)
    {
        AccountMFAJson? json = await rest.GetAsync<AccountMFAJson>("auth/mfa");
        if (json == null)
            return null;

        return new AccountMFA(json);
    }
}
