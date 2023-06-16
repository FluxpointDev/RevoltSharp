using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events;

internal class ServerDeleteEventJson
{
    [JsonProperty("id")]
    public string? ServerId;
}
