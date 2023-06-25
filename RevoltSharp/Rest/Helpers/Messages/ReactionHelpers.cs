using RevoltSharp.Rest;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevoltSharp;

/// <summary>
/// Revolt http/rest methods for reactions.
/// </summary>
public static class ReactionHelpers
{
    /// <inheritdoc cref="AddMessageReactionAsync(RevoltRestClient, string, string, string)" />
    public static Task AddReactionAsync(this UserMessage message, Emoji emoji)
        => AddMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emoji.Id);

    /// <inheritdoc cref="AddMessageReactionAsync(RevoltRestClient, string, string, string)" />
    public static Task AddReactionAsync(this UserMessage message, string emojiId)
        => AddMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emojiId);

    /// <summary>
    /// Add a reaction to the message.
    /// </summary>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task AddMessageReactionAsync(this RevoltRestClient rest, string channelId, string messageId, string emojiId)
    {
        Conditions.ChannelIdLength(channelId, nameof(AddMessageReactionAsync));
        Conditions.MessageIdLength(messageId, nameof(AddMessageReactionAsync));
        Conditions.EmojiIdLength(emojiId, nameof(AddMessageReactionAsync));

        await rest.PutAsync<HttpResponseMessage>($"channels/{channelId}/messages/{messageId}/reactions/{emojiId}");
    }

    /// <inheritdoc cref="RemoveMessageReactionAsync(RevoltRestClient, string, string, string, string, bool)" />
    public static Task RemoveReactionAsync(this UserMessage message, Emoji emoji, string userId, bool removeAll = false)
        => RemoveMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emoji.Id, userId, removeAll);

    /// <inheritdoc cref="RemoveMessageReactionAsync(RevoltRestClient, string, string, string, string, bool)" />
    public static Task RemoveReactionAsync(this UserMessage message, Emoji emoji, User user, bool removeAll = false)
        => RemoveMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emoji.Id, user.Id, removeAll);

    /// <inheritdoc cref="RemoveMessageReactionAsync(RevoltRestClient, string, string, string, string, bool)" />
    public static Task RemoveReactionAsync(this UserMessage message, string emojiId, User user, bool removeAll = false)
        => RemoveMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emojiId, user.Id, removeAll);

    /// <inheritdoc cref="RemoveMessageReactionAsync(RevoltRestClient, string, string, string, string, bool)" />
    public static Task RemoveReactionAsync(this UserMessage message, string emojiId, string userId, bool removeAll = false)
        => RemoveMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emojiId, userId, removeAll);

    /// <summary>
    /// Remove a reaction from the message.
    /// </summary>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task RemoveMessageReactionAsync(this RevoltRestClient rest, string channelId, string messageId, string emojiId, string userId, bool removeAll = false)
    {
        Conditions.ChannelIdLength(channelId, nameof(RemoveMessageReactionAsync));
        Conditions.MessageIdLength(messageId, nameof(RemoveMessageReactionAsync));
        Conditions.EmojiIdLength(emojiId, nameof(RemoveMessageReactionAsync));

        if (!removeAll)
            Conditions.UserIdLength(userId, nameof(RemoveMessageReactionAsync));


        await rest.DeleteAsync($"channels/{channelId}/messages/{messageId}/reactions/{emojiId}?" +
            $"user_id=" + userId + "&remove_all=" + removeAll.ToString());
    }

    /// <inheritdoc cref="RemoveAllMessageReactionsAsync(RevoltRestClient, string, string)" />
    public static Task RemoveAllReactionsAsync(this UserMessage message)
        => RemoveAllMessageReactionsAsync(message.Client.Rest, message.Channel.Id, message.Id);

    /// <summary>
    /// Remove all reactions from a message.
    /// </summary>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task RemoveAllMessageReactionsAsync(this RevoltRestClient rest, string channelId, string messageId)
    {
        Conditions.ChannelIdLength(channelId, nameof(RemoveAllMessageReactionsAsync));
        Conditions.MessageIdLength(messageId, nameof(RemoveAllMessageReactionsAsync));

        await rest.DeleteAsync($"channels/{channelId}/messages/{messageId}/reactions");
    }
}
