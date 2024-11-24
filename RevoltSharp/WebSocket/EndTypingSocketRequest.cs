using Newtonsoft.Json;

namespace RevoltSharp.WebSocket;

internal class EndTypingSocketRequest
{
    internal EndTypingSocketRequest(string channelId)
    {
        ChannelId = channelId;
    }

    [JsonProperty("type")]
    public string Type = "BeginTyping";

    [JsonProperty("channel")]
    public string ChannelId;
}