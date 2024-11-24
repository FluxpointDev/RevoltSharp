using Newtonsoft.Json;

namespace RevoltSharp.WebSocket;

internal class BeginTypingSocketRequest
{
    internal BeginTypingSocketRequest(string channelId)
    {
        ChannelId = channelId;
    }

    [JsonProperty("type")]
    public string Type = "BeginTyping";

    [JsonProperty("channel")]
    public string ChannelId;
}