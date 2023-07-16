using Newtonsoft.Json;

namespace RevoltSharp.WebSocket;


internal class ServerRoleEventsJson
{
    [JsonProperty("id")]
    public string? Id;

    [JsonProperty("role_id")]
    public string? RoleId;
}
internal class ServerRoleUpdateEventJson
{
    [JsonProperty("id")]
    public string? ServerId;

    [JsonProperty("role_id")]
    public string? RoleId;

    [JsonProperty("data")]
    public PartialRoleJson? Data;
}