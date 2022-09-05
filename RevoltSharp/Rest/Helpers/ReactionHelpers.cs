using RevoltSharp.Rest.Requests;
using RevoltSharp.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
            if (string.IsNullOrEmpty(channelId))
                throw new RevoltArgumentException("Channel id can't be empty for this request.");

            if (string.IsNullOrEmpty(messageId))
                throw new RevoltArgumentException("Message id can't be empty for this request.");

            if (string.IsNullOrEmpty(emojiId))
                throw new RevoltArgumentException("Emoji id can't be empty for this request.");

            return await rest.SendRequestAsync(RequestType.Put, $"channels/{channelId}/messages/{messageId}/reactions/{emojiId}");
        }

        public static Task<HttpResponseMessage> DeleteReactionAsync(this UserMessage message, Emoji emoji, string userId, bool removeAll = false)
            => DeleteMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emoji.Id, userId, removeAll);

        public static Task<HttpResponseMessage> DeleteReactionAsync(this UserMessage message, Emoji emoji, User user, bool removeAll = false)
            => DeleteMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emoji.Id, user.Id, removeAll);

        public static Task<HttpResponseMessage> DeleteReactionAsync(this UserMessage message, string emojiId, User user, bool removeAll = false)
            => DeleteMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emojiId, user.Id, removeAll);

        public static Task<HttpResponseMessage> DeleteReactionAsync(this UserMessage message, string emojiId, string userId, bool removeAll = false)
            => DeleteMessageReactionAsync(message.Client.Rest, message.Channel.Id, message.Id, emojiId, userId, removeAll);

        public static async Task<HttpResponseMessage> DeleteMessageReactionAsync(this RevoltRestClient rest, string channelId, string messageId, string emojiId, string userId, bool removeAll = false)
        {
            if (string.IsNullOrEmpty(channelId))
                throw new RevoltArgumentException("Channel id can't be empty for this request.");

            if (string.IsNullOrEmpty(messageId))
                throw new RevoltArgumentException("Message id can't be empty for this request.");

            if (!removeAll && string.IsNullOrEmpty(userId))
                throw new RevoltArgumentException("User id can't be empty for this request unless you specify remove all.");

            return await rest.SendRequestAsync(RequestType.Delete, $"channels/{channelId}/messages/{messageId}/reactions/{emojiId}", new DeleteReactionRequest
            {
                user_id = userId,
                remove_all = removeAll
            });
        }


        public static Task<HttpResponseMessage> DeleteAllReactionsAsync(this UserMessage message)
            => DeleteAllMessageReactionsAsync(message.Client.Rest, message.Channel.Id, message.Id);

        public static async Task<HttpResponseMessage> DeleteAllMessageReactionsAsync(this RevoltRestClient rest, string channelId, string messageId)
        {
            if (string.IsNullOrEmpty(channelId))
                throw new RevoltArgumentException("Channel id can't be empty for this request.");

            if (string.IsNullOrEmpty(messageId))
                throw new RevoltArgumentException("Message id can't be empty for this request.");

            return await rest.SendRequestAsync(RequestType.Delete, $"channels/{channelId}/messages/{messageId}/reactions");
        }

    }
}
