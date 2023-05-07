using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events;

internal class ChannelGroupLeaveEventJson
{
    [JsonProperty("id")]
    public string Id;

    [JsonProperty("user")]
    public string UserId;
}
