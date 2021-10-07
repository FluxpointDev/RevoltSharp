using System;
using System.Linq;

namespace RevoltSharp
{
    /// <summary>
    /// Revolt chat message with author, attachments, mentions and optional server.
    /// </summary>
    public abstract class Message : Entity
    {
        /// <summary>
        /// Id of the message.
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// Channel id of the message.
        /// </summary>
        public string ChannelId { get; internal set; }

        public Channel Channel { get; internal set; }

        /// <summary>
        /// Id of the user who posted the message.
        /// </summary>
        public string AuthorId { get; internal set; }

        public User Author { get; internal set; }

        public Message(RevoltClient client)
            : base(client)
        { }

        internal static Message Create(RevoltClient client, MessageJson json)
        {
            if (json.Author == "00000000000000000000000000")
                return new SystemMessage(client, json);

            return new UserMessage(client, json);
        }
    }
}
