using Newtonsoft.Json;
using Optionals;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using RevoltSharp.WebSocket;
using System.Threading.Tasks;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace RevoltSharp;
#pragma warning restore IDE0130 // Namespace does not match folder structure


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
        Conditions.ChannelIdLength(channelId, nameof(GetChannelAsync));

        if (rest.Client.TryGetChannel(channelId, out Channel chan))
            return (TValue)chan;

        ChannelJson? ChannelJson = await rest.GetAsync<ChannelJson>($"/channels/{channelId}");
        if (ChannelJson == null)
            return null;

        TValue Channel = (TValue)RevoltSharp.Channel.Create(rest.Client, ChannelJson);
        if (rest.Client.WebSocket != null)
            rest.Client.WebSocket.ChannelCache.TryAdd(channelId, Channel);

        return Channel;
    }


    /// <inheritdoc cref="ModifyChannelAsync(RevoltRestClient, string, Option{string}, Option{string}, Option{string}, Option{bool}, Option{string})" />
    public static Task<Channel> ModifyChannelAsync(this Server server, string channelId, Option<string>? name = null, Option<string?>? desc = null, Option<string?>? iconId = null, Option<bool>? nsfw = null)
        => InternalModifyChannelAsync<Channel>(server.Client.Rest, channelId, name, desc, iconId, nsfw, null);

    /// <inheritdoc cref="ModifyChannelAsync(RevoltRestClient, string, Option{string}, Option{string}, Option{string}, Option{bool}, Option{string})" />
    public static Task<Channel> ModifyChannelAsync(this RevoltRestClient rest, Channel channel, Option<string>? name = null, Option<string?>? desc = null, Option<string?>? iconId = null, Option<bool>? nsfw = null, Option<string>? owner = null)
        => InternalModifyChannelAsync<Channel>(rest, channel.Id, name, desc, iconId, nsfw, owner);

    /// <summary>
    /// Update a channel.
    /// </summary>
    /// <returns>
    /// <see cref="Channel"/>
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static Task<Channel> ModifyChannelAsync(this RevoltRestClient rest, string channelId, Option<string>? name = null, Option<string?>? desc = null, Option<string?>? iconId = null, Option<bool>? nsfw = null, Option<string>? owner = null)
        => InternalModifyChannelAsync<Channel>(rest, channelId, name, desc, iconId, nsfw, owner);

    internal static async Task<TChannel> InternalModifyChannelAsync<TChannel>(this RevoltRestClient rest, string channelId, Option<string>? name = null, Option<string?>? desc = null, Option<string?>? iconId = null, Option<bool>? nsfw = null, Option<string>? owner = null) where TChannel : Channel
    {
        Conditions.ChannelIdLength(channelId, nameof(ModifyChannelAsync));

        ModifyChannelRequest Req = new ModifyChannelRequest();
        if (name != null)
        {
            Conditions.ChannelNameLength(name.Value, nameof(ModifyChannelAsync));
            Req.name = Optional.Some(name.Value);
        }

        if (desc != null)
        {
            if (string.IsNullOrEmpty(desc.Value))
                Req.RemoveValue("Description");
            else
            {
                Conditions.ChannelDescriptionLength(desc.Value, nameof(ModifyChannelAsync));
                Req.description = Optional.Some(desc.Value);
            }
        }

        if (iconId != null)
        {
            if (string.IsNullOrEmpty(iconId.Value))
                Req.RemoveValue("Icon");
            else
            {
                Conditions.IconIdLength(owner.Value, nameof(ModifyChannelAsync));
                Req.icon = Optional.Some(iconId.Value);
            }

        }

        if (nsfw != null)
            Req.nsfw = Optional.Some(nsfw.Value);

        if (owner != null)
        {
            Conditions.OwnerIdLength(owner.Value, nameof(ModifyChannelAsync));
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
        Conditions.ChannelIdLength(channelId, nameof(DeleteChannelAsync));

        await rest.DeleteAsync($"/channels/{channelId}");
    }

    /// <inheritdoc cref="TriggerTypingChannelAsync(RevoltRestClient, string)" />
    public static Task TriggerTypingAsync(this Channel channel) => TriggerTypingChannelAsync(channel.Client.Rest, channel.Id);

    /// <inheritdoc cref="TriggerTypingChannelAsync(RevoltRestClient, string)" />
    public static Task TriggerTypingAsync(this RevoltRestClient rest, Channel channel) => TriggerTypingChannelAsync(rest, channel.Id);

    /// <summary>
    /// Trigger the typing indicator one-time on a channel which will stop after 3 seconds.
    /// </summary>
    /// <remarks>
    /// This will only work with <see cref="ClientMode.WebSocket"/>
    /// </remarks>
    public static async Task TriggerTypingChannelAsync(this RevoltRestClient rest, string channelId)
    {
        Conditions.ChannelIdLength(channelId, nameof(TriggerTypingChannelAsync));

        await rest.Client.WebSocket.Send(rest.Client.WebSocket.WebSocket, JsonConvert.SerializeObject(new BeginTypingSocketRequest(channelId)), new System.Threading.CancellationToken());
    }

    /// <inheritdoc cref="BeginTypingChannelAsync(RevoltRestClient, string)" />
    public static Task BeginTypingAsync(this Channel channel) => BeginTypingChannelAsync(channel.Client.Rest, channel.Id);

    /// <inheritdoc cref="BeginTypingChannelAsync(RevoltRestClient, string)" />
    public static Task BeginTypingAsync(this RevoltRestClient rest, Channel channel) => BeginTypingChannelAsync(rest, channel.Id);

    /// <summary>
    /// Trigger the typing indicator continuously on a channel, you can use the <see cref="TypingNotifier"/> class to stop typing or using the StopTypingChannelAsync rest/channel request.
    /// </summary>
    /// <remarks>
    /// This will only work with <see cref="ClientMode.WebSocket"/>
    /// </remarks>
    public static async Task<TypingNotifier> BeginTypingChannelAsync(this RevoltRestClient rest, string channelId)
    {
        Conditions.ChannelIdLength(channelId, nameof(BeginTypingChannelAsync));

        return new TypingNotifier(rest, channelId);
    }

    /// <inheritdoc cref="StopTypingChannelAsync(RevoltRestClient, string)" />
    public static Task StopTypingAsync(this Channel channel) => StopTypingChannelAsync(channel.Client.Rest, channel.Id);

    /// <inheritdoc cref="StopTypingChannelAsync(RevoltRestClient, string)" />
    public static Task StopTypingAsync(this RevoltRestClient rest, Channel channel) => StopTypingChannelAsync(rest, channel.Id);

    /// <summary>
    /// Trigger the stop typing on a channel which will stop the current user from typing in the channel.
    /// </summary>
    /// <remarks>
    /// This will only work with <see cref="ClientMode.WebSocket"/>
    /// </remarks>
    /// <param name="rest"></param>
    /// <param name="channelId"></param>
    public static async Task StopTypingChannelAsync(this RevoltRestClient rest, string channelId)
    {
        Conditions.ChannelIdLength(channelId, nameof(StopTypingChannelAsync));

        if (rest.Client.WebSocket.TypingChannels.TryGetValue(channelId, out TypingNotifier typing))
            typing.Stop();
        else
            await rest.Client.WebSocket.Send(rest.Client.WebSocket.WebSocket, JsonConvert.SerializeObject(new EndTypingSocketRequest(channelId)), new System.Threading.CancellationToken());
    }
}