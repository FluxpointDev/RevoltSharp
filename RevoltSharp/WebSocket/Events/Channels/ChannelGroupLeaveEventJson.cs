using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events;

internal class ChannelGroupLeaveEventJson
{
    [JsonProperty("id")]
    public string? ChannelId;

    [JsonProperty("user")]
    public string? UserId;
}
