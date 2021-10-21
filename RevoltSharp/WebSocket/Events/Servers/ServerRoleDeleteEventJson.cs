using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events
{
    internal class ServerRoleDeleteEventJson
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("roleId")]
        public string RoleId;
    }
}
