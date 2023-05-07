using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events;

internal class ChannelAckEventJson
{
    [JsonProperty("id")]
    public string Id;

    [JsonProperty("user")]
    public string UserId;

    [JsonProperty("messageId")]
    public string MessageId;
}
