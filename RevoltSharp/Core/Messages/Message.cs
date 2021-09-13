using System.Linq;

namespace RevoltSharp
{
    /// <summary>
    /// Revolt chat message with author, attachments, mentions and optional server.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Id of the message.
        /// </summary>
        public string Id { get; internal set; }
        /// <summary>
        /// Channel id of the message.
        /// </summary>
        public string ChannelId { get; internal set; }
        /// <summary>
        /// Server id of the message.
        /// </summary>
        public string ServerId { get; internal set; }
        /// <summary>
        /// Revolt server the message was posted to.
        /// </summary>
        /// <remarks>
        /// This will be <see langword="null"/> on <see cref="GroupChannel"/> and <see cref="ClientMode.Http"/>
        /// </remarks>
        public Server Server { get; internal set; }
        /// <summary>
        /// Revolt channel the message was posted to.
        /// </summary>
        /// <remarks>
        /// This will be <see langword="null"/> on <see cref="ClientMode.Http"/>
        /// </remarks>
        public Channel Channel { get; internal set; }
        /// <summary>
        /// Revolt user who posted the message.
        /// </summary>
        /// <remarks>
        /// This will be <see langword="null"/> on <see cref="ClientMode.Http"/>
        /// </remarks>
        public User Author { get; internal set; }
        /// <summary>
        /// Id of the user who posted the message.
        /// </summary>
        public string AuthorId { get; internal set; }
        /// <summary>
        /// The text content of the message.
        /// </summary>
        public string Content { get; internal set; }
        /// <summary>
        /// List of file attachments and images sent with the message.
        /// </summary>
        public Attachment[] Attachments { get; internal set; }
        /// <summary>
        /// List of mentions that this message has tagged.
        /// </summary>
        public string[] Mentions { get; internal set; }
        /// <summary>
        /// List of message replies this message is referencing.
        /// </summary>
        public string[] Replies { get; internal set; }

        internal static Message Create(RevoltClient client, MessageJson json)
        {
            Channel Chan = client.WebSocket != null ? client.WebSocket.ChannelCache[json.channel] : null;
            return new Message
            {
                Id = json.id,
                Author = client.WebSocket != null ? client.WebSocket.Usercache[json.author] : null,
                AuthorId = json.author,
                ChannelId = json.channel,
                Attachments = json.attachments != null ? json.attachments.Select(x => x.ToEntity()).ToArray() : new Attachment[0],
                Content = json.content,
                Mentions = json.mentions != null ? json.mentions : new string[0],
                Replies = json.replies != null ? json.replies : new string[0],
                Channel = Chan,
                ServerId = Chan.ServerId,
                Server = Chan.IsServer && client.WebSocket != null ? client.WebSocket.ServerCache[Chan.ServerId] : null
            };
        }
    }
}
