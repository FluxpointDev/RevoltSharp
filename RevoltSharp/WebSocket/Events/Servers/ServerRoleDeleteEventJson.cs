using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events;

internal class ServerRoleDeleteEventJson
{
    [JsonProperty("id")]
    public string? Id;

    [JsonProperty("role_id")]
    public string? RoleId;
}
