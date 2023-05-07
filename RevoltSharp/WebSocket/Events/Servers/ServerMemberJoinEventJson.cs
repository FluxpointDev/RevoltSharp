using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events;

internal class ServerMemberJoinEventJson
{
    [JsonProperty("id")]
    public string Id;

    [JsonProperty("user")]
    public string UserId;
}
