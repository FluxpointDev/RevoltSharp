using System;
using System.Collections.Generic;
using System.Linq;

namespace RevoltSharp;


/// <summary>
/// User messages sent with text, author, attachments, embeds, replies, mentions and optional parent server.
/// </summary>
public class UserMessage : Message
{
    /// <summary>
    /// File attachments for the message.
    /// </summary>
    public IReadOnlyList<Attachment> Attachments { get; internal set; }

    /// <summary>
    /// Mentions in this message.
    /// </summary>
    public IReadOnlyList<string> Mentions { get; internal set; }

    /// <summary>
    /// Replies in this message.
    /// </summary>
    public IReadOnlyList<string> Replies { get; internal set; }

    /// <summary>
    /// The date of the edited message or <see langword="null" />
    /// </summary>
    public DateTimeOffset? EditedAt { get; internal set; }

    /// <summary>
    /// Embeds in this message.
    /// </summary>
    public IReadOnlyList<MessageEmbed> Embeds { get; internal set; }

    /// <summary>
    /// Reactions on this message.
    /// </summary>
    public IReadOnlyDictionary<Emoji, User[]> Reactions { get; internal set; }

    /// <summary>
    /// The raw reaction type to user ids list.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> ReactionsRaw { get; internal set; }

    /// <summary>
    /// Masquerade options for this message.
    /// </summary>
    public MessageMasquerade? Masquerade { get; internal set; }

    //public MessageWebhook? Webhook { get; internal set; }

    internal UserMessage(RevoltClient client, MessageJson model, UserJson[]? users = null, ServerMemberJson[]? members = null)
        : base(client, model, users, members)
    {
        Content = model.Content;
        Masquerade = MessageMasquerade.Create(model.Masquerade);
        Attachments = model.Attachments == null ? new List<Attachment>() : new List<Attachment>(model.Attachments.Select(a => Attachment.Create(client, a)!));
        Mentions = model.Mentions == null ? new List<string>() : new List<string>(model.Mentions);
        Replies = model.Replies == null ? new List<string>() : new List<string>(model.Replies);
        //Webhook = model.Webhook != null ? new MessageWebhook(client, model.Webhook) : null;
        if (model.EditedAt.HasValue)
            EditedAt = model.EditedAt.Value;

        Embeds = model.Embeds == null ? new List<MessageEmbed>() : new List<MessageEmbed>(model.Embeds.Select(x => MessageEmbed.Create(client, x)!));

        if (!model.Reactions.HasValue)
        {
            Reactions = new Dictionary<Emoji, User[]>();
            ReactionsRaw = new Dictionary<string, string[]>();
        }
        else
        {
            // GetUser usually shouldn't return null here. If it can, change this from User[] to User?[].
            Dictionary<Emoji, User[]> react = new Dictionary<Emoji, User[]>();
            Dictionary<string, string[]> react_count = new Dictionary<string, string[]>();
            foreach (KeyValuePair<string, string[]> r in model.Reactions.Value)
            {
                react_count.Add(r.Key.Trim(), r.Value);
                var Emoji = Client.GetEmoji(r.Key);
                if (Emoji == null)
                    continue;

                try
                {
                    react.Add(Emoji, r.Value.Select(x => Client.GetUser(x)).ToArray()!);
                }
                catch { }
            }
            Reactions = react;
            ReactionsRaw = react_count;
        }
    }

    /// <summary> Returns a string that represents the current object.</summary>
    /// <returns> User message </returns>
    public override string ToString()
    {
        return Content ?? "";
    }
}