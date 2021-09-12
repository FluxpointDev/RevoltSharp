using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events
{
    internal class ChannelAckEventJson
    {
        public string id;
        [JsonProperty("user")]
        public string user_id;
        public string message_id;
    }
}
