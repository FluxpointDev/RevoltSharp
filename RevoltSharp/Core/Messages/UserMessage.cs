using System;
using System.Collections.Generic;

namespace RevoltSharp
{
    public class UserMessage : Message
    {
        public string Nonce { get; }

        public string Content { get; }

        public IReadOnlyList<Attachment> Attachments { get; }

        public IReadOnlyList<string> Mentions { get; }

        public IReadOnlyList<string> Replies { get; }

        public DateTimeOffset? EditedAt { get; }

        public IReadOnlyList<Embed> Embeds { get; }

        public UserMessage(RevoltClient client, MessageJson model)
            : base(client)
        {
            Id = model.Id;
            AuthorId = model.Author;
            ChannelId = model.Channel;
            Nonce = model.Nonce;
            Content = model.Content as string;
        }
    }
}