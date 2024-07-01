using Newtonsoft.Json;
using Optionals;
using System;

namespace RevoltSharp;


internal class ServerMemberJson
{
    [JsonProperty("_id")]
    public ServerMemberIdsJson Id { get; set; } = null!;

    [JsonProperty("nickname")]
    public string Nickname { get; set; } = null!;

    [JsonProperty("avatar")]
    public AttachmentJson? Avatar { get; set; }

    [JsonProperty("roles")]
    public string[]? Roles { get; set; }

    [JsonProperty("joined_at")]
    public DateTime JoinedAt { get; set; }

    [JsonProperty("timeout")]
    public Optional<DateTime> Timeout { get; set; }
}
internal class ServerMemberIdsJson
{
    [JsonProperty("server")]
    public string Server { get; set; } = null!;

    [JsonProperty("user")]
    public string User { get; set; } = null!;
}