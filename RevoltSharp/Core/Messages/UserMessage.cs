using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
            Author = client.GetUser(AuthorId);
            ChannelId = model.Channel;
            Channel = client.GetChannel(ChannelId);
            Nonce = model.Nonce;
            Content = model.Content as string;
            Attachments = new List<Attachment>(model.Attachments.Select(a => new Attachment(client, a)));
            Mentions = new List<string>(model.Mentions);
            Replies = new List<string>(model.Replies);
        }
    }
}