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
        public static Task<UserMessage> SendMessageAsync(this Channel channel, string text, Embed[] embeds = null, string[] attachments = null, MessageMasquerade masquerade = null, MessageInteractions interactions = null, MessageReply[] replies = null)
            => SendMessageAsync(channel.Client.Rest, channel.Id, text, embeds, attachments, masquerade, interactions, replies);

        public static async Task<UserMessage> SendMessageAsync(this RevoltRestClient rest, string channelId, string text, Embed[] embeds = null, string[] attachments = null, MessageMasquerade masquerade = null, MessageInteractions interactions = null, MessageReply[] replies = null)
        {
            Conditions.ChannelIdEmpty(channelId, "SendMessageAsync");

            if (string.IsNullOrEmpty(text) && (attachments == null || attachments.Length == 0) && (embeds == null || embeds.Length == 0))
                throw new RevoltArgumentException("Message content, attachments and embed can't be empty on SendMessageAsync");

            if (text.Length > 2000)
                throw new RevoltArgumentException("Message content can't be more than 2000 on SendMessageAsync");

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
            if (string.IsNullOrEmpty(text))
                text = null;

            MessageJson Data = await rest.SendRequestAsync<MessageJson>(RequestType.Post, $"channels/{channelId}/messages", new SendMessageRequest
            {
                content = Optional.Some(text),
                nonce = Optional.Some(Guid.NewGuid().ToString()),
                attachments = attachments == null ? Optional.None<string[]>() : Optional.Some(attachments),
                embeds = embeds == null ? Optional.None<EmbedJson[]>() : Optional.Some(embeds.Select(x => x.ToJson()).ToArray()),
                masquerade = masquerade == null ? Optional.None<MessageMasqueradeJson>() : Optional.Some(masquerade.ToJson()),
                interactions = interactions == null ? Optional.None<MessageInteractionsJson>() : Optional.Some(new MessageInteractionsJson
                {
                    reactions = interactions.Reactions == null ? new string[0] : interactions.Reactions.Select(x => x.Id).ToArray(),
                    restrict_reactions = interactions.RestrictReactions
                }),
                replies = replies == null ? Optional.None<MessageReply[]>() : Optional.Some(replies),
            });
            return Message.Create(rest.Client, Data) as UserMessage;
        }

        public static Task<UserMessage> SendFileAsync(this Channel channel, string filePath, string text = null, Embed[] embeds = null, MessageMasquerade masquerade = null, MessageInteractions interactions = null, MessageReply[] replies = null)
        => SendFileAsync(channel.Client.Rest, channel.Id, System.IO.File.ReadAllBytes(filePath), filePath.Split('/').Last().Split('\\').Last(), text, embeds, masquerade, interactions, replies);

        public static Task<UserMessage> SendFileAsync(this Channel channel, byte[] bytes, string fileName, string text = null, Embed[] embeds = null, MessageMasquerade masquerade = null, MessageInteractions interactions = null, MessageReply[] replies = null)
        => SendFileAsync(channel.Client.Rest, channel.Id, bytes, fileName, text, embeds, masquerade, interactions, replies);

        public static Task<UserMessage> SendFileAsync(this RevoltRestClient rest, string channelId, string filePath, string text = null, Embed[] embeds = null, MessageMasquerade masquerade = null, MessageInteractions interactions = null, MessageReply[] replies = null)
        => SendFileAsync(rest, channelId, System.IO.File.ReadAllBytes(filePath), filePath.Split('/').Last().Split('\\').Last(), text, embeds, masquerade, interactions, replies);

        public static async Task<UserMessage> SendFileAsync(this RevoltRestClient rest, string channelId, byte[] bytes, string fileName, string text = null, Embed[] embeds = null, MessageMasquerade masquerade = null, MessageInteractions interactions = null, MessageReply[] replies = null)
        {
            Conditions.FileBytesEmpty(bytes, "SendFileAsync");
            Conditions.FileNameEmpty(fileName, "SendFileAsync");

            FileAttachment File = await rest.UploadFileAsync(bytes, fileName, Rest.RevoltRestClient.UploadFileType.Attachment);
            return await rest.SendMessageAsync(channelId, text, embeds, new string[] { File.Id }, masquerade, interactions, replies).ConfigureAwait(false);
        }

        public static Task<IEnumerable<Message>> GetMessagesAsync(this Channel channel, int messageCount = 100, bool includeUserDetails = false, string beforeMessageId = "", string afterMessageId = "")
            => GetMessagesAsync(channel.Client.Rest, channel.Id, messageCount, includeUserDetails, beforeMessageId, afterMessageId);

        public static async Task<IEnumerable<Message>> GetMessagesAsync(this RevoltRestClient rest, string channelId, int messageCount = 100, bool includeUserDetails = false, string beforeMessageId = "", string afterMessageId = "")
        {
            Conditions.ChannelIdEmpty(channelId, "GetMessagesAsync");

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
            Conditions.ChannelIdEmpty(channelId, "GetMessageAsync");
            Conditions.MessageIdEmpty(messageId, "GetMessageAsync");

            MessageJson Data = await rest.SendRequestAsync<MessageJson>(RequestType.Get, $"channels/{channelId}/messages/{messageId}");
            return Message.Create(rest.Client, Data);
        }

        public static Task<UserMessage> EditMessageAsync(this UserMessage msg, Option<string> content, Option<Embed[]> embeds = null)
            => EditMessageAsync(msg.Client.Rest, msg.ChannelId, msg.Id, content, embeds);

        public static async Task<UserMessage> EditMessageAsync(this RevoltRestClient rest, string channelId, string messageId, Option<string> content, Option<Embed[]> embeds = null)
        {
            Conditions.ChannelIdEmpty(channelId, "EditMessageAsync");
            Conditions.MessageIdEmpty(messageId, "EditMessageAsync");

            var Req = new SendMessageRequest();
            if (content != null)
                Req.content = Optional.Some(content.Value);
            if (embeds != null)
                Req.embeds = Optional.Some(embeds.Value.Select(x => x.ToJson()).ToArray());
            MessageJson Data = await rest.SendRequestAsync<MessageJson>(RequestType.Patch, $"channels/{channelId}/messages/{messageId}", Req);
            return Message.Create(rest.Client, Data) as UserMessage;
        }


        public static Task<HttpResponseMessage> DeleteMessageAsync(this Message mes)
          => DeleteMessageAsync(mes.Channel.Client.Rest, mes.ChannelId, mes.Id);

        public static Task<HttpResponseMessage> DeleteMessageAsync(this Channel channel, Message message)
            => DeleteMessageAsync(channel.Client.Rest, channel.Id, message.Id);

        public static Task<HttpResponseMessage> DeleteMessageAsync(this Channel channel, string messageId)
            => DeleteMessageAsync(channel.Client.Rest, channel.Id, messageId);

        public static async Task<HttpResponseMessage> DeleteMessageAsync(this RevoltRestClient rest, string channelId, string messageId)
        {
            Conditions.ChannelIdEmpty(channelId, "DeleteMessageAsync");
            Conditions.MessageIdEmpty(messageId, "DeleteMessageAsync");


            return await rest.SendRequestAsync(RequestType.Delete, $"channels/{channelId}/messages/{messageId}");
        }

        
    }
}
