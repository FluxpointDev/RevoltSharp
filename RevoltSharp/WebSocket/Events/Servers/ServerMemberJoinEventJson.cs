using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events
{
    internal class ServerMemberJoinEventJson
    {
        public string id;
        [JsonProperty("user")]
        public string user_id;
    }
}
