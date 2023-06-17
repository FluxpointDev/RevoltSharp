using System;
using System.Collections.Generic;
using System.Linq;

namespace RevoltSharp;

/// <summary>
/// User messages sent with text, author, attachments, embeds, replies, mentions and optional parent server.
/// </summary>
public class UserMessage : Message
{
    public string? Nonce { get; internal set; }

    public string? Content { get; internal set; }

    public IReadOnlyList<Attachment> Attachments { get; internal set; }

    public IReadOnlyList<string> Mentions { get; internal set; }

    public IReadOnlyList<string> Replies { get; internal set; }

    public DateTimeOffset? EditedAt { get; internal set; }

    public IReadOnlyList<MessageEmbed> Embeds { get; internal set; }

    public IReadOnlyDictionary<Emoji, User[]> Reactions { get; internal set; }

    public MessageMasquerade? Masquerade { get; internal set; }

    internal UserMessage(RevoltClient client, MessageJson model)
        : base(client, model)
    {
        Nonce = model.Nonce;
        Content = model.Content;
        Masquerade = MessageMasquerade.Create(model.Masquerade);
        Attachments = model.Attachments == null ? new List<Attachment>() : new List<Attachment>(model.Attachments.Select(a => Attachment.Create(client, a)!));
        Mentions = model.Mentions == null ? new List<string>() : new List<string>(model.Mentions);
        Replies = model.Replies == null ? new List<string>() : new List<string>(model.Replies);
        if (model.Edited.HasValue)
            EditedAt = model.Edited.Value;
        Embeds = model.Embeds == null ? new List<MessageEmbed>() : new List<MessageEmbed>(model.Embeds.Select(x => MessageEmbed.Create(x)!));

        if (!model.Reactions.HasValue)
        {
            Reactions = new Dictionary<Emoji, User[]>();
        }
        else
        {
            // GetUser usually shouldn't return null here. If it can, change this from User[] to User?[].
            Dictionary<Emoji, User[]> react = new Dictionary<Emoji, User[]>();
            foreach (KeyValuePair<string, string[]> r in model.Reactions.Value)
                react.Add(Client.GetEmoji(r.Key), r.Value.Select(x => Client.GetUser(x)).ToArray()!);
            Reactions = react;
        }
    }
}