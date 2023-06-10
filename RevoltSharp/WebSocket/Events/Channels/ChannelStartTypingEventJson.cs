using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events;

internal class ChannelStartTypingEventJson
{
    [JsonProperty("id")]
    public string? Id;

    [JsonProperty("user")]
    public string? UserId;
}
