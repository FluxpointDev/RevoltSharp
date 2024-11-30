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
}
