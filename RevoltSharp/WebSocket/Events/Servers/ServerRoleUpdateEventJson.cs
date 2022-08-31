using Newtonsoft.Json;
using Optional;

namespace RevoltSharp.WebSocket.Events
{
    internal class ServerRoleUpdateEventJson
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("role_id")]
        public string RoleId;

        [JsonProperty("data")]
        public PartialRoleJson Data;

        [JsonProperty("clear")]
        public Option<string[]> Clear;
    }
}
