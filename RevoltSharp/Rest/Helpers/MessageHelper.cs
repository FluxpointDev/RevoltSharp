using RevoltSharp.Rest.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RevoltSharp.Rest
{
    public static class MessageHelper
    {
        public static async Task<Message> SendMessageAsync(this RevoltRestClient rest, string channelId, string content, string[] attachments = null)
        {
            MessageJson Data = await rest.SendRequestAsync<MessageJson>(RequestType.Post, $"channels/{channelId}/messages", new SendMessageRequest
            { 
                content = content,
                nonce = Guid.NewGuid().ToString(),
                attachments = attachments
            });
            return Message.Create(rest.Client, Data);
        }

        public static async Task<IEnumerable<Message>> GetMessagesAsync(this RevoltRestClient rest, string channelId, GetMessagesRequest req)
        {
            MessageJson[] Data = await rest.SendRequestAsync<MessageJson[]>(RequestType.Get, $"channels/{channelId}/messages", req);

            return Data.Select(x => Message.Create(rest.Client, x));
        }

        public static async Task<Message> GetMessageAsync(this RevoltRestClient rest, string channelId, string messageId)
        {
            MessageJson Data = await rest.SendRequestAsync<MessageJson>(RequestType.Get, $"channels/{channelId}/messages/{messageId}", null);
           
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(Data, Newtonsoft.Json.Formatting.Indented));
            return Message.Create(rest.Client, Data);
        }

        public static async Task EditMessageAsync(this RevoltRestClient rest, string channelId, string messageId, string content)
        {
            await rest.SendRequestAsync(RequestType.Patch, $"channels/{channelId}/messages/{messageId}", new SendMessageRequest
            {
                content = content
            });
        }


        public static Task DeleteMessageAsync(this Message mes)
          => DeleteMessageAsync(mes.Channel.Client.Rest, mes.ChannelId, mes.Id);

        public static async Task DeleteMessageAsync(this RevoltRestClient rest, string channelId, string messageId)
        {
            await rest.SendRequestAsync(RequestType.Delete, $"channels/{channelId}/messages/{messageId}");
        }
    }
}
