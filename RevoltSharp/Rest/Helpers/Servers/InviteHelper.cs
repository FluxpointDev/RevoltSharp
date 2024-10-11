using RevoltSharp.Rest;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace RevoltSharp;
#pragma warning restore IDE0130 // Namespace does not match folder structure


/// <summary>
/// Revolt http/rest methods for server invites.
/// </summary>
public static class InviteHelper
{
    /// <inheritdoc cref="DeleteInviteAsync(RevoltRestClient, string)" />
    public static Task DeleteAsync(this Invite invite)
        => DeleteInviteAsync(invite.Client.Rest, invite.ChannelId);

    /// <inheritdoc cref="DeleteInviteAsync(RevoltRestClient, string)" />
    public static Task DeleteInviteAsync(this Server server, Invite invite)
        => DeleteInviteAsync(server.Client.Rest, invite.Code);

    /// <inheritdoc cref="DeleteInviteAsync(RevoltRestClient, string)" />
    public static Task DeleteInviteAsync(this Server server, string inviteCode)
        => DeleteInviteAsync(server.Client.Rest, inviteCode);

    /// <inheritdoc cref="DeleteInviteAsync(RevoltRestClient, string)" />
    public static Task DeleteInviteAsync(this RevoltRestClient rest, Invite invite)
        => DeleteInviteAsync(rest, invite.Code);

    /// <summary>
    /// Delete an invite.
    /// </summary>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task DeleteInviteAsync(this RevoltRestClient rest, string inviteCode)
    {
        Conditions.InviteCodeEmpty(inviteCode, nameof(DeleteInviteAsync));

        await rest.DeleteAsync($"/invites/{inviteCode}");
    }

    /// <inheritdoc cref="GetInvitesAsync(RevoltRestClient, string)" />
    public static Task<IReadOnlyCollection<Invite>?> GetInvitesAsync(this Server server)
        => GetInvitesAsync(server.Client.Rest, server.Id);

    /// <summary>
    /// Get a list of invites for the server.
    /// </summary>
    /// <returns>
    /// List of <see cref="Invite"/>
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<IReadOnlyCollection<Invite>?> GetInvitesAsync(this RevoltRestClient rest, string serverId)
    {
        Conditions.ServerIdLength(serverId, nameof(GetInvitesAsync));

        InviteJson[]? Json = await rest.GetAsync<InviteJson[]>($"/servers/{serverId}/invites");
        if (Json == null)
            return System.Array.Empty<Invite>();

        return Json.Select(x => new Invite(rest.Client, x)).ToImmutableArray();
    }

    /// <inheritdoc cref="GetInviteAsync(RevoltRestClient, string)" />
    public static Task<Invite?> GetInviteAsync(this Server server, string inviteCode)
        => GetInviteAsync(server.Client.Rest, inviteCode);

    /// <summary>
    /// Get info for an invite.
    /// </summary>
    /// <returns>
    /// <see cref="CreatedInvite"/>
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<Invite?> GetInviteAsync(this RevoltRestClient rest, string inviteCode)
    {
        Conditions.InviteCodeEmpty(inviteCode, nameof(GetInviteAsync));

        InviteJson? Json = await rest.GetAsync<InviteJson>($"/invites/{inviteCode}");
        if (Json == null)
            return null;
        return new Invite(rest.Client, Json);
    }



    /// <inheritdoc cref="CreateInviteAsync(RevoltRestClient, string)" />
    public static Task<CreatedInvite> CreateInviteAsync(this TextChannel channel)
        => CreateInviteAsync(channel.Client.Rest, channel.Id);

    /// <summary>
    /// Only user accounts can create invites
    /// </summary>
    /// <returns>
    /// <see cref="CreatedInvite"/>
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<CreatedInvite> CreateInviteAsync(this RevoltRestClient rest, string channelId)
    {
        Conditions.ChannelIdLength(channelId, nameof(CreateInviteAsync));

        CreateInviteJson Json = await rest.PostAsync<CreateInviteJson>($"/channels/{channelId}/invites");
        return new CreatedInvite(rest.Client, Json);
    }
}