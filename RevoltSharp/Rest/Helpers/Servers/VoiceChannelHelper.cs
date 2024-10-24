﻿using Optionals;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System.Threading.Tasks;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace RevoltSharp;
#pragma warning restore IDE0130 // Namespace does not match folder structure


/// <summary>
/// Revolt http/rest methods for voice channel.
/// </summary>
public static class VoiceChannelHelper
{
    /// <summary>
    /// Delete this voice channel.
    /// </summary>
    /// <inheritdoc cref="ChannelHelper.DeleteChannelAsync(RevoltRestClient, string)" />
    public static Task DeleteAsync(this VoiceChannel channel)
        => ChannelHelper.DeleteChannelAsync(channel.Client.Rest, channel.Id);

    /// <inheritdoc cref="GetVoiceChannelAsync(RevoltRestClient, string)" />
    public static Task<VoiceChannel?> GetVoiceChannelAsync(this Server server, string channelId)
        => ChannelHelper.InternalGetChannelAsync<VoiceChannel>(server.Client.Rest, channelId);

    /// <summary>
    /// Get a server voice channel.
    /// </summary>
    /// <returns>
    /// <see cref="VoiceChannel"/> or <see langword="null" />
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static Task<VoiceChannel?> GetVoiceChannelAsync(this RevoltRestClient rest, string channelId)
        => ChannelHelper.InternalGetChannelAsync<VoiceChannel>(rest, channelId);

    /// <inheritdoc cref="CreateVoiceChannelAsync(RevoltRestClient, string, string, string)" />
    public static Task<VoiceChannel> CreateVoiceChannelAsync(this Server server, string name, string? description = null)
        => CreateVoiceChannelAsync(server.Client.Rest, server.Id, name, description);

    /// <inheritdoc cref="CreateVoiceChannelAsync(RevoltRestClient, string, string, string)" />
    public static Task<VoiceChannel> CreateVoiceChannelAsync(this RevoltRestClient rest, Server server, string name, string? description = null)
        => CreateVoiceChannelAsync(rest, server.Id, name, description);

    
    /// <summary>
    /// Create a server voice channel with properties.
    /// </summary>
    /// <returns>
    /// <see cref="VoiceChannel" />
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<VoiceChannel> CreateVoiceChannelAsync(this RevoltRestClient rest, string serverId, string name, string? description = null)
    {
        Conditions.ServerIdLength(serverId, nameof(CreateVoiceChannelAsync));
        Conditions.ChannelNameLength(name, nameof(CreateVoiceChannelAsync));

        CreateChannelRequest Req = new CreateChannelRequest
        {
            name = name,
            type = Optional.Some("Voice")
        };
        if (!string.IsNullOrEmpty(description))
        {
            Conditions.ChannelDescriptionLength(description, nameof(CreateVoiceChannelAsync));
            Req.description = Optional.Some(description);
        }

        ChannelJson Json = await rest.PostAsync<ChannelJson>($"/servers/{serverId}/channels", Req);
        return new VoiceChannel(rest.Client, Json);
    }

    /// <summary>
    /// Update a voice channel.
    /// </summary>
    /// <returns>
    /// <see cref="VoiceChannel"/>
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static Task<VoiceChannel> ModifyAsync(this VoiceChannel channel, Option<string>? name = null, Option<string?>? desc = null, Option<string?>? iconId = null, Option<bool>? nsfw = null)
        => ChannelHelper.InternalModifyChannelAsync<VoiceChannel>(channel.Client.Rest, channel.Id, name, desc, iconId, nsfw, null);


}