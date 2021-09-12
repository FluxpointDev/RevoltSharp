using System.Linq;

namespace RevoltSharp
{
    public class Message
    {
        public string Id { get; internal set; }
        public string ChannelId { get; internal set; }
        public Server Server { get; internal set; }
        public Channel Channel { get; internal set; }
        public string AuthorId { get; internal set; }
        public string Content { get; internal set; }
        public Attachment[] Attachments { get; internal set; }
        public string[] Mentions { get; internal set; }
        public string[] Replies { get; internal set; }

        internal static Message Create(RevoltClient client, MessageJson json)
        {
            Channel Chan = client.WebSocket != null ? client.WebSocket.ChannelCache[json.channel] : null;

            return new Message
            {
                Id = json.id,
                AuthorId = json.author,
                ChannelId = json.channel,
                Attachments = json.attachments != null ? json.attachments.Select(x => x.ToEntity()).ToArray() : new Attachment[0],
                Content = json.content,
                Mentions = json.mentions != null ? json.mentions : new string[0],
                Replies = json.replies != null ? json.replies : new string[0],
                Channel = Chan
            };
        }
    }
}
