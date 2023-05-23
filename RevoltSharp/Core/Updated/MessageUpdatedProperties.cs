using Optionals;
using RevoltSharp.WebSocket;
using RevoltSharp.WebSocket.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevoltSharp;

public class MessageUpdatedProperties : CreatedEntity
{
    internal MessageUpdatedProperties(RevoltClient client, MessageUpdateEventJson json) : base(client, json.Id)
    {
        Content = json.Data.Content;
        if (json.Data.Embeds.HasValue)
        {
            if (json.Data.Embeds.Value == null || !json.Data.Embeds.Value.Any())
                Embeds = new Option<MessageEmbed[]>(Array.Empty<MessageEmbed>());
            else
                Embeds = new Option<MessageEmbed[]>(json.Data.Embeds.Value.Select(x => MessageEmbed.Create(x)).ToArray());
        }
        Edited = json.Data.Edited;
        ChannelId = json.Channel;
        if (Channel is ServerChannel SC)
            ServerId = SC.ServerId;
    }

    public string Id { get; private set; }

    public Optional<string> Content { get; private set; }

    public Option<MessageEmbed[]> Embeds { get; private set; }

    public DateTime Edited { get; private set; }

    public string ChannelId { get; private set; }

    public Channel? Channel => Client.GetChannel(ChannelId);

    public string ServerId { get; private set; }

    public Server? Server => Client.GetServer(ServerId);

}
