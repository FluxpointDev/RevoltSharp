using RevoltSharp.Rest;
using System.Linq;
using System.Threading.Tasks;

namespace RevoltSharp;

public static class InviteHelper
{
    public static Task DeleteAsync(this Invite invite)
        => DeleteInviteAsync(invite.Client.Rest, invite.ChannelId);

    public static Task DeleteInviteAsync(this Server server, Invite invite)
        => DeleteInviteAsync(server.Client.Rest, invite.Code);

    public static Task DeleteInviteAsync(this Server server, string inviteCode)
        => DeleteInviteAsync(server.Client.Rest, inviteCode);

    public static Task DeleteInviteAsync(this RevoltRestClient rest, Invite invite)
        => DeleteInviteAsync(rest, invite.Code);

    public static async Task DeleteInviteAsync(this RevoltRestClient rest, string inviteCode)
    {
        Conditions.InviteCodeEmpty(inviteCode, "DeleteInviteAsync");

        await rest.DeleteAsync($"/invites/{inviteCode}");
    }

    public static Task<Invite[]?> GetInvitesAsync(this Server server)
        => GetInvitesAsync(server.Client.Rest, server.Id);

    public static async Task<Invite[]?> GetInvitesAsync(this RevoltRestClient rest, string serverId)
    {
        Conditions.ServerIdEmpty(serverId, "GetInvitesAsync");

        InviteJson[]? Json = await rest.GetAsync<InviteJson[]>($"/servers/{serverId}/invites");
        if (Json == null)
            return null;

        return Json.Select(x => new Invite(rest.Client, x)).ToArray();
    }

    public static Task<Invite?> GetInviteAsync(this Server server, string inviteCode)
        => GetInviteAsync(server.Client.Rest, inviteCode);

    public static async Task<Invite?> GetInviteAsync(this RevoltRestClient rest, string inviteCode)
    {
        Conditions.InviteCodeEmpty(inviteCode, "GetInviteAsync");

        InviteJson? Json = await rest.GetAsync<InviteJson>($"/invites/{inviteCode}");
        if (Json == null)
            return null;
        return new Invite(rest.Client, Json);
    }



    /// <summary>
    /// Only user accounts can create invites
    /// </summary>
    /// <param name="channel"></param>
    /// <returns></returns>
    public static Task<CreatedInvite> CreateInviteAsync(this TextChannel channel)
        => CreateInviteAsync(channel.Client.Rest, channel.Id);

    /// <summary>
    /// Only user accounts can create invites
    /// </summary>
    /// <param name="rest"></param>
    /// <param name="channelId"></param>
    /// <returns></returns>
    /// <exception cref="RevoltArgumentException"></exception>
    public static async Task<CreatedInvite> CreateInviteAsync(this RevoltRestClient rest, string channelId)
    {
        Conditions.ChannelIdEmpty(channelId, "CreateInviteAsync");

        CreateInviteJson Json = await rest.PostAsync<CreateInviteJson>($"/channels/{channelId}/invites");
        return new CreatedInvite(rest.Client, Json);
    }
}
