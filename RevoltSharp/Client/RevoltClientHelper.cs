namespace RevoltSharp;

/// <summary>
/// Revolt client methods.
/// </summary>
public static class RevoltClientHelper
{
    /// <summary>
    /// Get a server <see cref="Role" /> from the websocket cache.
    /// </summary>
    /// <returns>
    /// <see cref="Role" /> or <see langword="null" />
    /// </returns>
    public static Role? GetRole(this RevoltClient client, string roleId)
    {
        if (client.WebSocket != null && !string.IsNullOrEmpty(roleId))
        {
            foreach (Server s in client.WebSocket.ServerCache.Values)
            {
                Role role = s.GetRole(roleId);
                if (role != null)
                    return role;
            }
        }
        return null;
    }

    /// <inheritdoc cref="GetRole(RevoltClient, string)" />
    public static bool TryGetRole(this RevoltClient client, string roleId, out Role role)
    {
        role = GetRole(client, roleId);
        return role != null;
    }

    /// <summary>
    /// Get a server <see cref="Emoji" /> from the websocket cache.
    /// </summary>
    /// <returns><see cref="Emoji" /> or <see langword="null" /></returns>
    public static Emoji? GetEmoji(this RevoltClient client, string emojiId)
    {
        if (client.WebSocket != null && !string.IsNullOrEmpty(emojiId))
        {
            if (client.WebSocket.EmojiCache.TryGetValue(emojiId, out Emoji emoji))
                return emoji;
        }
        return null;
    }

    /// <inheritdoc cref="GetEmoji(RevoltClient, string)" />
    public static bool TryGetEmoji(this RevoltClient client, string emojiId, out Emoji emoji)
    {
        emoji = GetEmoji(client, emojiId);
        return emoji != null;
    }

    /// <summary>
    /// Get a server <see cref="TextChannel" /> from the websocket cache.
    /// </summary>
    /// <returns><see cref="TextChannel" /> or <see langword="null" /></returns>
    public static TextChannel? GetTextChannel(this RevoltClient client, string channelId)
    {
        if (client.WebSocket != null && !string.IsNullOrEmpty(channelId) && client.WebSocket.ChannelCache.TryGetValue(channelId, out Channel Chan) && Chan is TextChannel TC)
            return TC;
        return null;
    }

    /// <inheritdoc cref="GetTextChannel(RevoltClient, string)" />
    public static bool TryGetTextChannel(this RevoltClient client, string channelId, out TextChannel channel)
    {
        channel = GetTextChannel(client, channelId);
        return channel != null;
    }

    /// <summary>
    /// Get a server <see cref="VoiceChannel" /> from the websocket cache.
    /// </summary>
    /// <returns><see cref="VoiceChannel" /> or <see langword="null" /></returns>
    public static VoiceChannel? GetVoiceChannel(this RevoltClient client, string channelId)
    {
        if (client.WebSocket != null && !string.IsNullOrEmpty(channelId) && client.WebSocket.ChannelCache.TryGetValue(channelId, out Channel Chan) && Chan is VoiceChannel VC)
            return VC;
        return null;
    }

    /// <inheritdoc cref="GetVoiceChannel(RevoltClient, string)" />
    public static bool TryGetVoiceChannel(this RevoltClient client, string channelId, out VoiceChannel channel)
    {
        channel = GetVoiceChannel(client, channelId);
        return channel != null;
    }

    /// <summary>
    /// Get a <see cref="Server" /> from the websocket cache.
    /// </summary>
    /// <returns><see cref="Server" /> or <see langword="null" /></returns>
    public static Server? GetServer(this RevoltClient client, string serverId)
    {
        if (client.WebSocket != null && !string.IsNullOrEmpty(serverId) && client.WebSocket.ServerCache.TryGetValue(serverId, out Server Server))
            return Server;
        return null;
    }

    /// <inheritdoc cref="GetServer(RevoltClient, string)" />
    public static bool TryGetServer(this RevoltClient client, string serverId, out Server server)
    {
        server = GetServer(client, serverId);
        return server != null;
    }

    /// <summary>
    /// Get a <see cref="User" /> from the websocket cache.
    /// </summary>
    /// <returns><see cref="User" /> or <see langword="null" /></returns>
    public static User? GetUser(this RevoltClient client, string userId)
    {
        if (client.WebSocket != null && !string.IsNullOrEmpty(userId) && client.WebSocket.UserCache.TryGetValue(userId, out User User))
            return User;
        return null;
    }

    /// <inheritdoc cref="GetUser(RevoltClient, string)" />
    public static bool TryGetUser(this RevoltClient client, string userId, out User user)
    {
        user = GetUser(client, userId);
        return user != null;
    }

    /// <summary>
    /// Get a <see cref="Channel" /> from the websocket cache.
    /// </summary>
    /// <returns><see cref="Channel" /> or <see langword="null" /></returns>
    public static Channel? GetChannel(this RevoltClient client, string channelId)
    {
        if (client.WebSocket != null && !string.IsNullOrEmpty(channelId) && client.WebSocket.ChannelCache.TryGetValue(channelId, out Channel Chan))
            return Chan;
        return null;
    }

    /// <inheritdoc cref="GetChannel(RevoltClient, string)" />
    public static bool TryGetChannel(this RevoltClient client, string channelId, out Channel channel)
    {
        channel = GetChannel(client, channelId);
        return channel != null;
    }

    /// <summary>
    /// Get a <see cref="GroupChannel" /> from the websocket cache.
    /// </summary>
    /// <returns><see cref="GroupChannel" /> or <see langword="null" /></returns>
    public static GroupChannel? GetGroupChannel(this RevoltClient client, string channelId)
    {
        if (client.WebSocket != null && !string.IsNullOrEmpty(channelId) && client.WebSocket.ChannelCache.TryGetValue(channelId, out Channel Chan) && Chan is GroupChannel GC)
            return GC;
        return null;
    }
    /// <inheritdoc cref="GetGroupChannel(RevoltClient, string)" />
    public static bool TryGetGroupChannel(this RevoltClient client, string channelId, out GroupChannel channel)
    {
        channel = GetGroupChannel(client, channelId);
        return channel != null;
    }

    /// <summary>
    /// Get a <see cref="DMChannel" /> from the websocket cache.
    /// </summary>
    /// <returns><see cref="DMChannel" /> or <see langword="null" /></returns>
    public static DMChannel? GetDMChannel(this RevoltClient client, string channelId)
    {
        if (client.WebSocket != null && !string.IsNullOrEmpty(channelId) && client.WebSocket.ChannelCache.TryGetValue(channelId, out Channel Chan) && Chan is DMChannel DM)
            return DM;
        return null;
    }

    /// <inheritdoc cref="GetDMChannel(RevoltClient, string)" />
    public static bool TryGetDMChannel(this RevoltClient client, string channelId, out DMChannel channel)
    {
        channel = GetDMChannel(client, channelId);
        return channel != null;
    }
}
