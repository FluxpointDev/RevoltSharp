using Newtonsoft.Json;
using Optionals;

namespace RevoltSharp.WebSocket.Events;

internal class ChannelUpdateEventJson
{
    [JsonProperty("id")]
    public string? ChannelId;

    [JsonProperty("data")]
    public PartialChannelJson? Data;

    [JsonProperty("clear")]
    public Optional<string[]> Clear;
}
