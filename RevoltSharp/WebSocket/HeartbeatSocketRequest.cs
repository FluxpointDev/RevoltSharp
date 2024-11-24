using Newtonsoft.Json;

namespace RevoltSharp.WebSocket;

internal class HeartbeatSocketRequest
{
    [JsonProperty("type")]
    public string Type = "Ping";

    [JsonProperty("data")]
    public int Data = 20000;
}
