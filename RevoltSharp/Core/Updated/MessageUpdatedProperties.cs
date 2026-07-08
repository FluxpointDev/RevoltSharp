using Optionals;
using StoatSharp.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StoatSharp;


public class MessageUpdatedProperties : CreatedEntity
{
    internal MessageUpdatedProperties(StoatClient client, MessageUpdateEventJson json) : base(client, json.MessageId)
    {
        Content = json.Data.Content;
        EditedAt = json.Data.EditedAt;
        ChannelId = json.ChannelId;
        Embeds = json.Data.Embeds.HasValue ? Optional.Some(json.Data.Embeds.Value.Select(x => MessageEmbed.Create(client, x)!).ToArray()) : Optional.None<MessageEmbed[]>();
        Reactions = json.Data.Reactions.HasValue ?
            Optional.Some<IReadOnlyDictionary<Emoji, User[]>>(json.Data.Reactions.Value.ToDictionary(x => new Emoji(client, x.Key),
                x => x.Value.Select(u => new User(client, u)).ToArray())) : Optional.None<IReadOnlyDictionary<Emoji, User[]>>();
        if (Channel is ServerChannel SC)
            ServerId = SC.ServerId;
    }

    /// <summary>
    /// Id of the message.
    /// </summary>
    public new string Id => base.Id;

    /// <summary>
    /// Date of when the message was created.
    /// </summary>
    public new DateTimeOffset CreatedAt => base.CreatedAt;

    public Optional<string> Content { get; private set; }

    public Optional<MessageEmbed[]> Embeds { get; private set; }

    public Optional<IReadOnlyDictionary<Emoji, User[]>> Reactions { get; private set; }

    public DateTime EditedAt { get; private set; }

    public string ChannelId { get; private set; }

    public Channel? Channel => Client.GetChannel(ChannelId);

    public string? ServerId { get; private set; }

    public Server? Server => Client.GetServer(ServerId);

}