using Newtonsoft.Json;
using Optionals;

namespace RevoltSharp.WebSocket.Events;

internal class ServerUpdateEventJson
{
    [JsonProperty("id")]
    public string? ServerId;

    [JsonProperty("data")]
    public PartialServerJson? Data;

    [JsonProperty("clear")]
    public Optional<string[]> Clear;
}
