using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events;

internal class MessageDeleteEventJson
{
    [JsonProperty("id")]
    public string Id;

    [JsonProperty("ids")]
    public string[] Ids;

    [JsonProperty("channel")]
    public string ChannelId;
}
