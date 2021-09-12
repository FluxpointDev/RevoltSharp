using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events
{
    internal class MessageDeleteEventJson
    {
        public string id;
        [JsonProperty("channel")]
        public string channel_id;
    }
}
