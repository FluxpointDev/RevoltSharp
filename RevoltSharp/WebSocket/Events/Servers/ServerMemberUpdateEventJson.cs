using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events
{
    internal class ServerMemberUpdateEventJson
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("data")]
        public ServerMemberJson Data;
    }
}
