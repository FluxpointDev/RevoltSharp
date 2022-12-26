using RevoltSharp.Rest.Requests;
using RevoltSharp.Rest;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public static class ReactionHelpers
    {
        public static Task<HttpResponseMessage> AddReactionAsync(this UserMessage message, Emoji emoji)
            => AddMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emoji.Id);
        public static Task<HttpResponseMessage> AddReactionAsync(this UserMessage message, string emojiId)
            => AddMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emojiId);

        public static async Task<HttpResponseMessage> AddMessageReactionAsync(this RevoltRestClient rest, string channelId, string messageId, string emojiId)
        {
            Conditions.ChannelIdEmpty(channelId);
            Conditions.MessageIdEmpty(messageId);
            Conditions.EmojiIdEmpty(emojiId);

            return await rest.SendRequestAsync(RequestType.Put, $"channels/{channelId}/messages/{messageId}/reactions/{emojiId}");
        }

        public static Task<HttpResponseMessage> RemoveReactionAsync(this UserMessage message, Emoji emoji, string userId, bool removeAll = false)
            => RemoveMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emoji.Id, userId, removeAll);

        public static Task<HttpResponseMessage> RemoveReactionAsync(this UserMessage message, Emoji emoji, User user, bool removeAll = false)
            => RemoveMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emoji.Id, user.Id, removeAll);

        public static Task<HttpResponseMessage> RemoveReactionAsync(this UserMessage message, string emojiId, User user, bool removeAll = false)
            => RemoveMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emojiId, user.Id, removeAll);

        public static Task<HttpResponseMessage> RemoveReactionAsync(this UserMessage message, string emojiId, string userId, bool removeAll = false)
            => RemoveMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emojiId, userId, removeAll);

        public static async Task<HttpResponseMessage> RemoveMessageReactionAsync(this RevoltRestClient rest, string channelId, string messageId, string emojiId, string userId, bool removeAll = false)
        {
            Conditions.ChannelIdEmpty(channelId);
            Conditions.MessageIdEmpty(messageId);

            if (!removeAll)
                Conditions.UserIdEmpty(userId);

            return await rest.SendRequestAsync(RequestType.Delete, $"channels/{channelId}/messages/{messageId}/reactions/{emojiId}", new DeleteReactionRequest
            {
                user_id = userId,
                remove_all = removeAll
            });
        }


        public static Task<HttpResponseMessage> RemoveAllReactionsAsync(this UserMessage message)
            => RemoveAllMessageReactionsAsync(message.Client.Rest, message.Channel.Id, message.Id);

        public static async Task<HttpResponseMessage> RemoveAllMessageReactionsAsync(this RevoltRestClient rest, string channelId, string messageId)
        {
            Conditions.ChannelIdEmpty(channelId);
            Conditions.MessageIdEmpty(messageId);

            return await rest.SendRequestAsync(RequestType.Delete, $"channels/{channelId}/messages/{messageId}/reactions");
        }

    }
}
