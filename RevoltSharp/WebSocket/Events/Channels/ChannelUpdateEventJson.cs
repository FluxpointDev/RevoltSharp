using Newtonsoft.Json;
using Optional;

namespace RevoltSharp.WebSocket.Events
{
    internal class ChannelUpdateEventJson
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("data")]
        public PartialChannelJson Data;

        [JsonProperty("clear")]
        public Option<string[]> Clear;
    }
}
