using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events
{
    internal class ServerMemberLeaveEventJson
    {
        public string id;
        [JsonProperty("user")]
        public string user_id;
    }
}
