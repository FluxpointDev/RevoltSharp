using Newtonsoft.Json;
using Optional;

namespace RevoltSharp.WebSocket.Events
{
    internal class ServerUpdateEventJson
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("data")]
        public PartialServerJson Data;

        [JsonProperty("clear")]
        public Option<string> Clear;
    }
}
