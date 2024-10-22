using Newtonsoft.Json;
using System.Collections.Generic;

namespace RevoltSharp;

public class ServerCategory : CreatedEntity
{
    internal ServerCategory(RevoltClient client, string serverId, CategoryJson model, int position) : base(client, model.id)
    {
        Name = model.title;
        ChannelIds = model.channels;
        ServerId = serverId;
        Position = position;
    }

    public string Name { get; internal set; }

    public string[] ChannelIds { get; internal set; }

    public string ServerId { get; internal set; }

    public int Position { get; internal set; }

    [JsonIgnore]
    public Server? Server => Client.GetServer(ServerId);

    [JsonIgnore]
    public IReadOnlyList<ServerChannel> Channels { get; internal set; } = new List<ServerChannel>();

    internal void Update(RevoltClient client, CategoryJson model, int position)
    {
        Name = model.title;
        ChannelIds = model.channels;
        Position = position;
        UpdateChannels(client);
    }

    internal void UpdateChannels(RevoltClient client)
    {
        if (client.WebSocket != null)
        {
            List<ServerChannel> channels = new List<ServerChannel>();
            foreach (var channel in ChannelIds)
            {
                if (client.TryGetChannel(channel, out var chan))
                    channels.Add(chan as ServerChannel);
            }
            Channels = channels;
        }
    }

    internal CategoryJson ToJson()
    {
        return new CategoryJson
        {
            id = Id,
            title = Name,
            channels = ChannelIds,
        };
    }
}
