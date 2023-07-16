using System;
using System.Collections.Generic;
using System.Linq;

namespace RevoltSharp;


/// <summary>
/// User messages sent with text, author, attachments, embeds, replies, mentions and optional parent server.
/// </summary>
public class UserMessage : Message
{
    //public string? Nonce { get; internal set; }

    /// <summary>
    /// Content of the message.
    /// </summary>
    public string? Content { get; internal set; }

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
    /// Masquerade options for this message.
    /// </summary>
    public MessageMasquerade? Masquerade { get; internal set; }

    //public MessageWebhook? Webhook { get; internal set; }
    internal UserMessage(RevoltClient client, MessageJson model)
        : base(client, model)
    {
        //Nonce = model.Nonce;
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
        }
        else
        {
            // GetUser usually shouldn't return null here. If it can, change this from User[] to User?[].
            Dictionary<Emoji, User[]> react = new Dictionary<Emoji, User[]>();
            foreach (KeyValuePair<string, string[]> r in model.Reactions.Value)
            {
                try
                {
                    react.Add(Client.GetEmoji(r.Key), r.Value.Select(x => Client.GetUser(x)).ToArray()!);
                }
                catch { }
            }
            Reactions = react;
        }
    }

    /// <summary> Returns a string that represents the current object.</summary>
    /// <returns> User message </returns>
    public override string ToString()
    {
        return Content ?? "";
    }
}