using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events
{
    internal class ChannelStopTypingEventJson
    {
        public string id;
        [JsonProperty("user")]
        public string user_id;
    }
}
