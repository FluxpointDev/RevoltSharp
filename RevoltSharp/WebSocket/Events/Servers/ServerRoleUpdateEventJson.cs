using Newtonsoft.Json;
using Optionals;

namespace RevoltSharp.WebSocket.Events;

internal class ServerRoleUpdateEventJson
{
    [JsonProperty("id")]
    public string ServerId;

    [JsonProperty("role_id")]
    public string RoleId;

    [JsonProperty("data")]
    public PartialRoleJson Data;
}
