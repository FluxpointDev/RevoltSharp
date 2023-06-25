using Optionals;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System.Threading.Tasks;

namespace RevoltSharp;

/// <summary>
/// Revolt http/rest methods for text channels.
/// </summary>
public static class TextChannelHelper
{
    /// <inheritdoc cref="GetTextChannelAsync(RevoltRestClient, string)" />
    public static Task<TextChannel?> GetTextChannelAsync(this Server server, string channelId)
        => ChannelHelper.InternalGetChannelAsync<TextChannel>(server.Client.Rest, channelId);

    /// <summary>
    /// Get a server text channel.
    /// </summary>
    /// <returns>
    /// <see cref="TextChannel"/> or <see langword="null" />
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static Task<TextChannel?> GetTextChannelAsync(this RevoltRestClient rest, string channelId)
        => ChannelHelper.InternalGetChannelAsync<TextChannel>(rest, channelId);

    /// <inheritdoc cref="CreateTextChannelAsync(RevoltRestClient, string, string, string, bool)" />
    public static Task<TextChannel> CreateTextChannelAsync(this Server server, string name, string description = null, bool nsfw = false)
        => InternalCreateTextChannelAsync(server.Client.Rest, server.Id, name, description, nsfw);

    /// <inheritdoc cref="CreateTextChannelAsync(RevoltRestClient, string, string, string, bool)" />
    public static Task<TextChannel> CreateTextChannelAsync(this RevoltRestClient rest, Server server, string name, string description = null, bool nsfw = false)
        => InternalCreateTextChannelAsync(rest, server.Id, name, description, nsfw);

    /// <summary>
    /// Create a server text channel with properties.
    /// </summary>
    /// <returns>
    /// <see cref="TextChannel" />
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static Task<TextChannel> CreateTextChannelAsync(this RevoltRestClient rest, string serverId, string name, string description = null, bool nsfw = false)
        => InternalCreateTextChannelAsync(rest, serverId, name, description, nsfw);

    internal static async Task<TextChannel> InternalCreateTextChannelAsync(this RevoltRestClient rest, string serverId, string name, string description = null, bool nsfw = false)
    {
        Conditions.ServerIdLength(serverId, nameof(CreateTextChannelAsync));
        Conditions.ChannelNameLength(name, nameof(CreateTextChannelAsync));

        CreateChannelRequest Req = new CreateChannelRequest
        {
            name = name,
            type = Optional.Some("Text")
        };
        if (!string.IsNullOrEmpty(description))
        {
            Conditions.ChannelDescriptionLength(description, nameof(CreateTextChannelAsync));
			Req.description = Optional.Some(description);
		}

        if (nsfw)
            Req.nsfw = Optional.Some(true);

        ChannelJson Json = await rest.PostAsync<ChannelJson>($"/servers/{serverId}/channels", Req);
        return new TextChannel(rest.Client, Json);
    }

    /// <summary>
    /// Update a text channel.
    /// </summary>
    /// <returns>
    /// <see cref="TextChannel"/>
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static Task<TextChannel> ModifyAsync(this TextChannel channel, Option<string> name = null, Option<string> desc = null, Option<string> iconId = null, Option<bool> nsfw = null)
        => ChannelHelper.InternalModifyChannelAsync<TextChannel>(channel.Client.Rest, channel.Id, name, desc, iconId, nsfw, null);

}
