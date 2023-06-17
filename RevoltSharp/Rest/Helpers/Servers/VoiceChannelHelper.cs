using Optionals;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System.Threading.Tasks;

namespace RevoltSharp;

/// <summary>
/// Revolt http/rest methods for voice channel.
/// </summary>
public static class VoiceChannelHelper
{
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
    public static Task<VoiceChannel> CreateVoiceChannelAsync(this Server server, string name, string description = null)
        => InternalCreateVoiceChannelAsync(server.Client.Rest, server.Id, name, description);

    /// <inheritdoc cref="CreateVoiceChannelAsync(RevoltRestClient, string, string, string)" />
    public static Task<VoiceChannel> CreateVoiceChannelAsync(this RevoltRestClient rest, Server server, string name, string description = null)
        => InternalCreateVoiceChannelAsync(rest, server.Id, name, description);

    /// <summary>
    /// Create a server voice channel with properties.
    /// </summary>
    /// <returns>
    /// <see cref="VoiceChannel" />
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static Task<VoiceChannel> CreateVoiceChannelAsync(this RevoltRestClient rest, string serverId, string name, string description = null)
        => InternalCreateVoiceChannelAsync(rest, serverId, name, description);

    internal static async Task<VoiceChannel> InternalCreateVoiceChannelAsync(this RevoltRestClient rest, string serverId, string name, string description = null)
    {
        Conditions.ServerIdEmpty(serverId, nameof(CreateVoiceChannelAsync));
        Conditions.ChannelNameEmpty(name, nameof(CreateVoiceChannelAsync));

        CreateChannelRequest Req = new CreateChannelRequest
        {
            name = name,
            type = Optional.Some("Voice")
        };
        if (!string.IsNullOrEmpty(description))
            Req.description = Optional.Some(description);

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
    public static Task<VoiceChannel> ModifyAsync(this VoiceChannel channel, Option<string> name = null, Option<string> desc = null, Option<string> iconId = null, Option<bool> nsfw = null)
        => ChannelHelper.InternalModifyChannelAsync<VoiceChannel>(channel.Client.Rest, channel.Id, name, desc, iconId, nsfw, null);


}
