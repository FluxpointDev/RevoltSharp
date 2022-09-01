using Optional.Unsafe;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevoltSharp
{
    /// <summary>
    /// User messages sent with text, author, attachments, embeds, replies, mentions and optional parent server.
    /// </summary>
    public class UserMessage : Message
    {
        public string Nonce { get; internal set; }

        public string Content { get; internal set; }

        public IReadOnlyList<Attachment> Attachments { get; internal set; }

        public IReadOnlyList<string> Mentions { get; internal set; }

        public IReadOnlyList<string> Replies { get; internal set; }

        public DateTimeOffset? EditedAt { get; internal set; }

        public IReadOnlyList<Embed> Embeds { get; internal set; }

        internal UserMessage(RevoltClient client, MessageJson model)
            : base(client)
        {
            Id = model.Id;
            AuthorId = model.Author;
            Author = client.GetUser(model.Author);
            ChannelId = model.Channel;
            Channel = client.GetChannel(model.Channel);
            Nonce = model.Nonce;
            Content = model.Content as string;
            Attachments = model.Attachments == null ? new List<Attachment>() : new List<Attachment>(model.Attachments.Select(a => new Attachment(client, a)));
            Mentions = model.Mentions == null ? new List<string>() : new List<string>(model.Mentions);
            Replies = model.Replies == null ? new List<string>() : new List<string>(model.Replies);
            if (model.Edited.HasValue)
                EditedAt = model.Edited.ValueOrDefault();
            Embeds = model.Embeds == null ? new List<Embed>() : new List<Embed>(model.Embeds);
        }
    }
}