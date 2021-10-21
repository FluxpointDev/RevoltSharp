using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events
{
    internal class MessageUpdateEventJson
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("channel")]
        public string Channel;

        [JsonProperty("data")]
        public MessageJson Data;
    }
}
