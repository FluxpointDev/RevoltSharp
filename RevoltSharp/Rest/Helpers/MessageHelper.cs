using Optionals;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public static class MessageHelper
    {
        public static Task<Message> SendMessageAsync(this Channel channel, string content, string[] attachments = null, Embed[] embeds = null, MessageMasquerade masquerade = null, MessageInteractions interactions = null, MessageReply[] replies = null)
            => SendMessageAsync(channel.Client.Rest, channel.Id, content, attachments, embeds, masquerade, interactions, replies);

        public static async Task<Message> SendMessageAsync(this RevoltRestClient rest, string channelId, string content, string[] attachments = null, Embed[] embeds = null, MessageMasquerade masquerade = null, MessageInteractions interactions = null, MessageReply[] replies = null)
        {
            Conditions.ChannelIdEmpty(channelId);

            if (string.IsNullOrEmpty(content) && (attachments == null || attachments.Length == 0) && (embeds == null || embeds.Length == 0))
                throw new RevoltArgumentException("Message content, attachments and embed can't be empty.");

            if (content.Length > 2000)
                throw new RevoltArgumentException("Message content can't be more than 2000");

            if (rest.Client.UserBot && embeds != null)
                throw new RevoltRestException("Userbots cannot send embeds!", 401);

            if (embeds != null && embeds.Any(x => !string.IsNullOrEmpty(x.Image)))
            {
                var uploadTasks = embeds.Select(async x =>
                {
                    if (x.Image.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || x.Image.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    {
                        var Bytes = await rest.FileHttpClient.GetByteArrayAsync(x.Image);
                        var Upload = await rest.UploadFileAsync(Bytes, "image.png", RevoltRestClient.UploadFileType.Attachment);
                        x.Image = Upload.Id;
                    }
                    else if (x.Image.Contains('/') || x.Image.Contains('\\'))
                    {
                        if (!System.IO.File.Exists(x.Image))
                            throw new RevoltArgumentException("Embed image url path does not exist.");
                        var Upload = await rest.UploadFileAsync(x.Image, RevoltRestClient.UploadFileType.Attachment);
                        x.Image = Upload.Id;
                    }

                });
                await Task.WhenAll(uploadTasks);
            }
            MessageJson Data = await rest.SendRequestAsync<MessageJson>(RequestType.Post, $"channels/{channelId}/messages", new SendMessageRequest
            {
                content = Optional.Some(content),
                nonce = Optional.Some(Guid.NewGuid().ToString()),
                attachments = attachments == null ? Optional.None<string[]>() : Optional.Some(attachments),
                embeds = embeds == null ? Optional.None<EmbedJson[]>() : Optional.Some(embeds.Select(x => x.ToJson()).ToArray()),
                masquerade = masquerade == null ? Optional.None<MessageMasqueradeJson>() : Optional.Some<MessageMasqueradeJson>(masquerade.ToJson()),
                interactions = interactions == null ? Optional.None<MessageInteractionsJson>() : Optional.Some(new MessageInteractionsJson
                {
                    reactions = interactions.Reactions.Select(x => x.Id).ToArray(),
                    restrict_reactions = interactions.RestrictReactions
                }),
                replies = replies == null ? Optional.None<MessageReply[]>() : Optional.Some(replies),
            });
            return Message.Create(rest.Client, Data);
        }

        public static Task<IEnumerable<Message>> GetMessagesAsync(this Channel channel, int messageCount = 100, bool includeUserDetails = false, string beforeMessageId = "", string afterMessageId = "")
            => GetMessagesAsync(channel.Client.Rest, channel.Id, messageCount);

        public static async Task<IEnumerable<Message>> GetMessagesAsync(this RevoltRestClient rest, string channelId, int messageCount = 100, bool includeUserDetails = false, string beforeMessageId = "", string afterMessageId = "")
        {
            Conditions.ChannelIdEmpty(channelId);

            GetMessagesRequest Req = new GetMessagesRequest
            {
                limit = messageCount,
                include_users = includeUserDetails
            };
            if (!string.IsNullOrEmpty(afterMessageId))
                Req.after = new Optional<string>(afterMessageId);
            if (!string.IsNullOrEmpty(beforeMessageId))
                Req.after = new Optional<string>(beforeMessageId);
            MessageJson[] Data = await rest.SendRequestAsync<MessageJson[]>(RequestType.Get, $"channels/{channelId}/messages", Req);

            return Data.Select(x => Message.Create(rest.Client, x));
        }

        public static Task<Message> GetMessageAsync(this Channel channel, string messageId)
            => GetMessageAsync(channel.Client.Rest, channel.Id, messageId);

        public static async Task<Message> GetMessageAsync(this RevoltRestClient rest, string channelId, string messageId)
        {
            Conditions.ChannelIdEmpty(channelId);
            Conditions.MessageIdEmpty(messageId);

            MessageJson Data = await rest.SendRequestAsync<MessageJson>(RequestType.Get, $"channels/{channelId}/messages/{messageId}");
            return Message.Create(rest.Client, Data);
        }

        public static Task<Message> EditMessageAsync(this Message msg, Option<string> content, Option<Embed[]> embeds = null)
            => EditMessageAsync(msg.Client.Rest, msg.ChannelId, msg.Id, content, embeds);

        public static async Task<Message> EditMessageAsync(this RevoltRestClient rest, string channelId, string messageId, Option<string> content, Option<Embed[]> embeds = null)
        {
            Conditions.ChannelIdEmpty(channelId);
            Conditions.MessageIdEmpty(messageId);

            var Req = new SendMessageRequest();
            if (content != null)
                Req.content = Optional.Some(content.Value);
            if (embeds != null)
                Req.embeds = Optional.Some(embeds.Value.Select(x => x.ToJson()).ToArray());
            MessageJson Data = await rest.SendRequestAsync<MessageJson>(RequestType.Patch, $"channels/{channelId}/messages/{messageId}", Req);
            return Message.Create(rest.Client, Data);
        }


        public static Task<HttpResponseMessage> DeleteMessageAsync(this Message mes)
          => DeleteMessageAsync(mes.Channel.Client.Rest, mes.ChannelId, mes.Id);

        public static Task<HttpResponseMessage> DeleteMessageAsync(this Channel channel, Message message)
            => DeleteMessageAsync(channel.Client.Rest, channel.Id, message.Id);

        public static Task<HttpResponseMessage> DeleteMessageAsync(this Channel channel, string messageId)
            => DeleteMessageAsync(channel.Client.Rest, channel.Id, messageId);

        public static async Task<HttpResponseMessage> DeleteMessageAsync(this RevoltRestClient rest, string channelId, string messageId)
        {
            Conditions.ChannelIdEmpty(channelId);
            Conditions.MessageIdEmpty(messageId);


            return await rest.SendRequestAsync(RequestType.Delete, $"channels/{channelId}/messages/{messageId}");
        }

        
    }
}
