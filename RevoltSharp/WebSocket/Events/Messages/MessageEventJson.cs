using System;
using System.Linq;

namespace RevoltSharp.WebSocket.Events
{
    internal class MessageEventJson : MessageJson
    {
        internal Message ToEntity(RevoltClient client)
        {
            Channel Chan = client.WebSocket.ChannelCache[channel];
          
            return new Message
            {
                Id = id,
                Attachments = attachments != null ? attachments.Select(x => x.ToEntity()).ToArray() : new Attachment[0],
                AuthorId = author,
                Channel = Chan,
                ChannelId = channel,
                Content = content,
                Mentions = mentions,
                Replies = replies,
                Server = Chan.IsServer ? client.WebSocket.ServerCache[Chan.ServerId] : null
            };
        }
    }
}
