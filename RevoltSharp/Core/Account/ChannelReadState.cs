using System;

namespace RevoltSharp;
public class ChannelReadState
{
    internal ChannelReadState(ChannelReadStateJson json)
    {
        ChannelId = json._id.channel;
        LastMessageId = json.last_id;
        Mentions = json.mentions != null ? json.mentions : Array.Empty<string>();
    }

    public string ChannelId { get; internal set; }
    public string? LastMessageId { get; internal set; }
    public string[] Mentions { get; internal set; }
}
