using Optional;
using Optional.Collections;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace RevoltSharp
{
    public static class MessageHelper
    {
        public static Task<Message> SendMessageAsync(this Channel channel, string content, string[] attachments = null, Embed[] embeds = null, MessageMasquerade masquerade = null)
            => SendMessageAsync(channel.Client.Rest, channel.Id, content, attachments, embeds, masquerade);

        public static async Task<Message> SendMessageAsync(this RevoltRestClient rest, string channelId, string content, string[] attachments = null, Embed[] embeds = null, MessageMasquerade masquerade = null)
        {
            if (string.IsNullOrEmpty(channelId))
                throw new RevoltArgumentException("Channel id can't be empty for this request.");

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
                content = Option.Some(content),
                nonce = Option.Some(Guid.NewGuid().ToString()),
                attachments = attachments == null ? Option.None<string[]>() : Option.Some(attachments),
                embeds = embeds == null ? Option.None<EmbedJson[]>() : Option.Some(embeds.Select(x => x.ToJson()).ToArray()),
                masquerade = masquerade == null ? Option.None<MessageMasqueradeJson>() : Option.Some<MessageMasqueradeJson>(masquerade.ToJson())
            });
            return Message.Create(rest.Client, Data);
        }

        public static Task<IEnumerable<Message>> GetMessagesAsync(this Channel channel, string messageId, GetMessagesRequest req)
            => GetMessagesAsync(channel.Client.Rest, channel.Id, req);

        public static async Task<IEnumerable<Message>> GetMessagesAsync(this RevoltRestClient rest, string channelId, GetMessagesRequest req)
        {
            if (string.IsNullOrEmpty(channelId))
                throw new RevoltArgumentException("Channel id can't be empty for this request.");

            MessageJson[] Data = await rest.SendRequestAsync<MessageJson[]>(RequestType.Get, $"channels/{channelId}/messages", req);

            return Data.Select(x => Message.Create(rest.Client, x));
        }

        public static Task<Message> GetMessageAsync(this Channel channel, string messageId)
            => GetMessageAsync(channel.Client.Rest, channel.Id, messageId);

        public static async Task<Message> GetMessageAsync(this RevoltRestClient rest, string channelId, string messageId)
        {
            if (string.IsNullOrEmpty(channelId))
                throw new RevoltArgumentException("Channel id can't be empty for this request.");

            if (string.IsNullOrEmpty(messageId))
                throw new RevoltArgumentException("Message id can't be empty for this request.");

            MessageJson Data = await rest.SendRequestAsync<MessageJson>(RequestType.Get, $"channels/{channelId}/messages/{messageId}");
            return Message.Create(rest.Client, Data);
        }

        public static Task<Message> EditMessageAsync(this Message msg, Optional<string> content, Optional<Embed[]> embeds = null)
            => EditMessageAsync(msg.Client.Rest, msg.ChannelId, msg.Id, content, embeds);

        public static async Task<Message> EditMessageAsync(this RevoltRestClient rest, string channelId, string messageId, Optional<string> content, Optional<Embed[]> embeds = null)
        {
            if (string.IsNullOrEmpty(channelId))
                throw new RevoltArgumentException("Channel id can't be empty for this request.");

            if (string.IsNullOrEmpty(messageId))
                throw new RevoltArgumentException("Message id can't be empty for this request.");

            var Req = new SendMessageRequest();
            if (content != null)
                Req.content = Option.Some(content.Value);
            if (embeds != null)
                Req.embeds = Option.Some(embeds.Value.Select(x => x.ToJson()).ToArray());
            return await rest.SendRequestAsync<Message>(RequestType.Patch, $"channels/{channelId}/messages/{messageId}", Req);
        }


        public static Task<HttpResponseMessage> DeleteMessageAsync(this Message mes)
          => DeleteMessageAsync(mes.Channel.Client.Rest, mes.ChannelId, mes.Id);

        public static Task<HttpResponseMessage> DeleteMessageAsync(this Channel channel, Message message)
            => DeleteMessageAsync(channel.Client.Rest, channel.Id, message.Id);

        public static Task<HttpResponseMessage> DeleteMessageAsync(this Channel channel, string messageId)
            => DeleteMessageAsync(channel.Client.Rest, channel.Id, messageId);

        public static async Task<HttpResponseMessage> DeleteMessageAsync(this RevoltRestClient rest, string channelId, string messageId)
        {
            if (string.IsNullOrEmpty(channelId))
                throw new RevoltArgumentException("Channel id can't be empty for this request.");

            if (string.IsNullOrEmpty(messageId))
                throw new RevoltArgumentException("Message id can't be empty for this request.");


            return await rest.SendRequestAsync(RequestType.Delete, $"channels/{channelId}/messages/{messageId}");
        }

        
    }
}
