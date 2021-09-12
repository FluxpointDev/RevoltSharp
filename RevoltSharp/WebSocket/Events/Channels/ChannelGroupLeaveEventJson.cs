using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events
{
    internal class ChannelGroupLeaveEventJson
    {
        public string id;
        [JsonProperty("user")]
        public string user_id;
    }
}
