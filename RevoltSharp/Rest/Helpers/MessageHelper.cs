using Optional;
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
        public static Task<Message> SendMessageAsync(this Channel channel, string content, string[] attachments = null, Embed[] embeds = null)
            => SendMessageAsync(channel.Client.Rest, channel.Id, content, attachments, embeds);

        public static async Task<Message> SendMessageAsync(this RevoltRestClient rest, string channelId, string content, string[] attachments = null, Embed[] embeds = null)
        {
            MessageJson Data = await rest.SendRequestAsync<MessageJson>(RequestType.Post, $"channels/{channelId}/messages", new SendMessageRequest
            { 
                content = Option.Some(content),
                nonce = Option.Some(Guid.NewGuid().ToString()),
                attachments = Option.Some(attachments),
                embeds = embeds == null ? Option.Some<EmbedJson[]>(null) : Option.Some(embeds.Select(x => x.ToJson()).ToArray())
            });
            return Message.Create(rest.Client, Data);
        }

        public static Task<IEnumerable<Message>> GetMessagesAsync(this Channel channel, string messageId, GetMessagesRequest req)
            => GetMessagesAsync(channel.Client.Rest, channel.Id, req);

        public static async Task<IEnumerable<Message>> GetMessagesAsync(this RevoltRestClient rest, string channelId, GetMessagesRequest req)
        {
            MessageJson[] Data = await rest.SendRequestAsync<MessageJson[]>(RequestType.Get, $"channels/{channelId}/messages", req);

            return Data.Select(x => Message.Create(rest.Client, x));
        }

        public static Task<Message> GetMessageAsync(this Channel channel, string messageId)
            => GetMessageAsync(channel.Client.Rest, channel.Id, messageId);

        public static async Task<Message> GetMessageAsync(this RevoltRestClient rest, string channelId, string messageId)
        {
            MessageJson Data = await rest.SendRequestAsync<MessageJson>(RequestType.Get, $"channels/{channelId}/messages/{messageId}");
            return Message.Create(rest.Client, Data);
        }

        public static Task<Message> EditMessageAsync(this Message msg, Optional<string> content, Optional<Embed[]> embeds = null)
            => EditMessageAsync(msg.Client.Rest, msg.ChannelId, msg.Id, content, embeds);

        public static async Task<Message> EditMessageAsync(this RevoltRestClient rest, string channelId, string messageId, Optional<string> content, Optional<Embed[]> embeds = null)
        {
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
            return await rest.SendRequestAsync(RequestType.Delete, $"channels/{channelId}/messages/{messageId}");
        }



    }
}
