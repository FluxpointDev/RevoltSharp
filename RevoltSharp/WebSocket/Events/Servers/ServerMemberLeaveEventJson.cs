using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events;

internal class ServerMemberLeaveEventJson
{
    [JsonProperty("id")]
    public string? ServerId;

    [JsonProperty("user")]
    public string? UserId;
}
