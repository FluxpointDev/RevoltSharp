using System;
using System.Linq;

namespace RevoltSharp.WebSocket.Events
{
    internal class MessageEventJson : MessageJson
    {
        internal Message ToEntity(RevoltClient client)
        {
            Channel Chan = client.WebSocket.ChannelCache[Channel];
          
            return new Message
            {
                Id = id,
                Attachments = Attachments != null ? Attachments.Select(x => x.ToEntity()).ToArray() : new Attachment[0],
                AuthorId = Author,
                Channel = Chan,
                ChannelId = Channel,
                Content = Content,
                Mentions = Mentions,
                Replies = Replies,
                Server = Chan.IsServer ? client.WebSocket.ServerCache[Chan.ServerId] : null
            };
        }
    }
}
