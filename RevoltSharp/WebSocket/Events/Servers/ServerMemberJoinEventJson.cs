using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events;

internal class ServerMemberJoinEventJson
{
    [JsonProperty("id")]
    public string? ServerId;

    [JsonProperty("user")]
    public string? UserId;
}
