using System;
using System.Linq;

namespace RevoltSharp.WebSocket.Events
{
    internal class MessageEventJson : MessageJson
    {
        internal Message ToEntity(RevoltClient client)
        {
            Channel Chan = client.WebSocket.ChannelCache[channel];
            client.WebSocket.Usercache.TryGetValue(author, out User User);
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
                Server = Chan.IsServer ? client.WebSocket.ServerCache[Chan.ServerId] : null,
                Author = User,
                Client = client,
                ServerId = Chan.ServerId
            };
        }
    }
}
