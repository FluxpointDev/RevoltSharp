using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events;

internal class ChannelGroupJoinEventJson
{
    [JsonProperty("id")]
    public string? ChannelId;

    [JsonProperty("user")]
    public string? UserId;
}
