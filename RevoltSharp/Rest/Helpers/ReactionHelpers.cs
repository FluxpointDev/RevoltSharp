using RevoltSharp.Rest;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevoltSharp;

public static class ReactionHelpers
{
    public static Task AddReactionAsync(this UserMessage message, Emoji emoji)
        => AddMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emoji.Id);
    public static Task AddReactionAsync(this UserMessage message, string emojiId)
        => AddMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emojiId);

    public static async Task AddMessageReactionAsync(this RevoltRestClient rest, string channelId, string messageId, string emojiId)
    {
        Conditions.ChannelIdEmpty(channelId, "AddMessageReactionAsync");
        Conditions.MessageIdEmpty(messageId, "AddMessageReactionAsync");
        Conditions.EmojiIdEmpty(emojiId, "AddMessageReactionAsync");

        await rest.PutAsync<HttpResponseMessage>($"channels/{channelId}/messages/{messageId}/reactions/{emojiId}");
    }

    public static Task RemoveReactionAsync(this UserMessage message, Emoji emoji, string userId, bool removeAll = false)
        => RemoveMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emoji.Id, userId, removeAll);

    public static Task RemoveReactionAsync(this UserMessage message, Emoji emoji, User user, bool removeAll = false)
        => RemoveMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emoji.Id, user.Id, removeAll);

    public static Task RemoveReactionAsync(this UserMessage message, string emojiId, User user, bool removeAll = false)
        => RemoveMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emojiId, user.Id, removeAll);

    public static Task RemoveReactionAsync(this UserMessage message, string emojiId, string userId, bool removeAll = false)
        => RemoveMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emojiId, userId, removeAll);

    public static async Task RemoveMessageReactionAsync(this RevoltRestClient rest, string channelId, string messageId, string emojiId, string userId, bool removeAll = false)
    {
        Conditions.ChannelIdEmpty(channelId, "RemoveMessageReactionAsync");
        Conditions.MessageIdEmpty(messageId, "RemoveMessageReactionAsync");
        Conditions.EmojiIdEmpty(emojiId, "RemoveMessageReactionAsync");

        if (!removeAll)
            Conditions.UserIdEmpty(userId, "RemoveMessageReactionAsync");


        await rest.DeleteAsync($"channels/{channelId}/messages/{messageId}/reactions/{emojiId}?" +
            $"user_id=" + userId + "&remove_all=" + removeAll.ToString());
    }


    public static Task RemoveAllReactionsAsync(this UserMessage message)
        => RemoveAllMessageReactionsAsync(message.Client.Rest, message.Channel.Id, message.Id);

    public static async Task RemoveAllMessageReactionsAsync(this RevoltRestClient rest, string channelId, string messageId)
    {
        Conditions.ChannelIdEmpty(channelId, "RemoveAllMessageReactionsAsync");
        Conditions.MessageIdEmpty(messageId, "RemoveAllMessageReactionsAsync");

        await rest.DeleteAsync($"channels/{channelId}/messages/{messageId}/reactions");
    }

}
