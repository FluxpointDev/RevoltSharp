using Optionals;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System.Threading.Tasks;

namespace RevoltSharp;

/// <summary>
/// Revolt http/rest methods for base channel types.
/// </summary>
public static class ChannelHelper
{
    /// <inheritdoc cref="GetChannelAsync(RevoltRestClient, string)" />
    public static Task<Channel?> GetChannelAsync(this Server server, string channelId)
        => InternalGetChannelAsync<Channel>(server.Client.Rest, channelId);

    /// <summary>
    /// Get a channel.
    /// </summary>
    /// <returns>
    /// <see cref="Channel"/> or <see langword="null" />
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static Task<Channel?> GetChannelAsync(this RevoltRestClient rest, string channelId)
        => InternalGetChannelAsync<Channel>(rest, channelId);

    internal static async Task<TValue?> InternalGetChannelAsync<TValue>(this RevoltRestClient rest, string channelId)
        where TValue : Channel
    {
        Conditions.ChannelIdEmpty(channelId, "GetChannelAsync");

        if (rest.Client.WebSocket != null)
        {
            if (rest.Client.WebSocket.ChannelCache.TryGetValue(channelId, out Channel chan))
                return (TValue)chan;
            return null;
        }


        ChannelJson? Channel = await rest.GetAsync<ChannelJson>($"/channels/{channelId}");
        if (Channel == null)
            return null;

        return (TValue)RevoltSharp.Channel.Create(rest.Client, Channel);
    }


    /// <inheritdoc cref="ModifyChannelAsync(RevoltRestClient, string, Option{string}, Option{string}, Option{string}, Option{bool}, Option{string})" />
    public static Task<Channel> ModifyChannelAsync(this Server server, string channelId, Option<string> name = null, Option<string> desc = null, Option<string> iconId = null, Option<bool> nsfw = null)
        => InternalModifyChannelAsync<Channel>(server.Client.Rest, channelId, name, desc, iconId, nsfw, null);

    /// <inheritdoc cref="ModifyChannelAsync(RevoltRestClient, string, Option{string}, Option{string}, Option{string}, Option{bool}, Option{string})" />
    public static Task<Channel> ModifyChannelAsync(this RevoltRestClient rest, Channel channel, Option<string> name = null, Option<string> desc = null, Option<string> iconId = null, Option<bool> nsfw = null, Option<string> owner = null)
        => InternalModifyChannelAsync<Channel>(rest, channel.Id, name, desc, iconId, nsfw, owner);

    /// <summary>
    /// Update a channel.
    /// </summary>
    /// <returns>
    /// <see cref="Channel"/>
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static Task<Channel> ModifyChannelAsync(this RevoltRestClient rest, string channelId, Option<string> name = null, Option<string> desc = null, Option<string> iconId = null, Option<bool> nsfw = null, Option<string> owner = null)
        => InternalModifyChannelAsync<Channel>(rest, channelId, name, desc, iconId, nsfw, owner);

    internal static async Task<TChannel> InternalModifyChannelAsync<TChannel>(this RevoltRestClient rest, string channelId, Option<string> name = null, Option<string> desc = null, Option<string> iconId = null, Option<bool> nsfw = null, Option<string> owner = null) where TChannel : Channel
    {
        Conditions.ChannelIdEmpty(channelId, "ModifyChannelAsync");

        ModifyChannelRequest Req = new ModifyChannelRequest();
        if (name != null)
        {
            Conditions.ChannelNameEmpty(name.Value, "ModifyChannelAsync");

            Req.name = Optional.Some(name.Value);
        }
        if (desc != null)
        {
            if (string.IsNullOrEmpty(desc.Value))
                Req.RemoveValue("Description");
            else
                Req.description = Optional.Some(desc.Value);
        }

        if (iconId != null)
        {
            if (string.IsNullOrEmpty(iconId.Value))
                Req.RemoveValue("Icon");
            else
                Req.icon = Optional.Some(iconId.Value);
        }

        if (nsfw != null)
            Req.nsfw = Optional.Some(nsfw.Value);

        if (owner != null)
        {
            Conditions.UserIdEmpty(owner.Value, "ModifyChannelAsync");
            Req.owner = Optional.Some(owner.Value);
        }
        ChannelJson Json = await rest.PatchAsync<ChannelJson>($"/channels/{channelId}", Req);
        return (TChannel)Channel.Create(rest.Client, Json);
    }

    /// <inheritdoc cref="DeleteChannelAsync(Server, string)" />
    public static Task DeleteAsync(this ServerChannel channel)
        => InternalDeleteChannelAsync(channel.Client.Rest, channel.Id);

    /// <summary>
    /// Delete a server channel.
    /// </summary>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static Task DeleteChannelAsync(this Server server, string channelId)
        => InternalDeleteChannelAsync(server.Client.Rest, channelId);

    /// <inheritdoc cref="DeleteChannelAsync(RevoltRestClient, string)" />
    public static Task DeleteChannelAsync(this RevoltRestClient rest, Channel channel)
        => InternalDeleteChannelAsync(rest, channel.Id);

    /// <summary>
    /// Delete a channel.
    /// </summary>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static Task DeleteChannelAsync(this RevoltRestClient rest, string channelId)
        => InternalDeleteChannelAsync(rest, channelId);

    internal static async Task InternalDeleteChannelAsync(this RevoltRestClient rest, string channelId)
    {
        Conditions.ChannelIdEmpty(channelId, "DeleteChannelAsync");

        await rest.DeleteAsync($"/channels/{channelId}");
    }
}
