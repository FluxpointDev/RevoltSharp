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

        public IReadOnlyList<MessageEmbed> Embeds { get; internal set; }

        public IReadOnlyDictionary<Emoji, User[]> Reactions { get; internal set; }

        public MessageMasquerade? Masquerade { get; internal set; }

        internal UserMessage(RevoltClient client, MessageJson model)
            : base(client)
        {
            Id = model.Id;
            AuthorId = model.Author;
            Author = client.GetUser(model.Author);
            ChannelId = model.Channel;
            Channel = client.GetChannel(model.Channel);
            if (Channel != null && Channel is ServerChannel SC)
                ServerId = SC.ServerId;
            Nonce = model.Nonce;
            Content = model.Content;
            Masquerade = model.Masquerade == null ? null : new MessageMasquerade(model.Masquerade);
            Attachments = model.Attachments == null ? new List<Attachment>() : new List<Attachment>(model.Attachments.Select(a => new Attachment(a)));
            Mentions = model.Mentions == null ? new List<string>() : new List<string>(model.Mentions);
            Replies = model.Replies == null ? new List<string>() : new List<string>(model.Replies);
            if (model.Edited.HasValue)
                EditedAt = model.Edited.Value;
            Embeds = model.Embeds == null ? new List<MessageEmbed>() : new List<MessageEmbed>(model.Embeds.Select(x => new MessageEmbed(x)));
            if (!model.Reactions.HasValue)
                Reactions = new Dictionary<Emoji, User[]>();
            else
            {
                Dictionary<Emoji, User[]> React = new Dictionary<Emoji, User[]>();
                foreach(var r in model.Reactions.Value)
                {
                    React.Add(Client.GetEmoji(r.Key), r.Value.Select(x => Client.GetUser(x)).ToArray());
                }
                Reactions = React;
            }
            
        }
    }
}