using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events;

internal class BulkEventsJson
{
    [JsonProperty("v")]
    public dynamic[]? Events;
}