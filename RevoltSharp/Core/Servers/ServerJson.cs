using Newtonsoft.Json;
using Optionals;
using System.Collections.Generic;

namespace RevoltSharp;


internal class ServerJson
{
    [JsonProperty("_id")]
    public string Id { get; set; } = null!;

    [JsonProperty("owner")]
    public string Owner { get; set; } = null!;

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("channels")]
    public string[] Channels { get; set; } = null!;

    [JsonProperty("categories")]
    public CategoryJson[]? Categories { get; set; }

    [JsonProperty("system_messages")]
    public ServerSystemMessagesJson? SystemMessages { get; set; }

    [JsonProperty("roles")]
    public Dictionary<string, RoleJson>? Roles { get; set; }

    [JsonProperty("default_permissions")]
    public ulong DefaultPermissions { get; set; }

    [JsonProperty("icon")]
    public AttachmentJson? Icon { get; set; }

    [JsonProperty("banner")]
    public AttachmentJson? Banner { get; set; }

    [JsonProperty("analytics")]
    public bool Analytics { get; set; }

    [JsonProperty("discoverable")]
    public bool Discoverable { get; set; }

    [JsonProperty("nsfw")]
    public bool Nsfw { get; set; }
}

internal class ServerSystemMessagesJson
{
    [JsonProperty("user_joined")]
    public Optional<string> UserJoined { get; set; }

    [JsonProperty("user_left")]
    public Optional<string> UserLeft { get; set; }

    [JsonProperty("user_kicked")]
    public Optional<string> UserKicked { get; set; }

    [JsonProperty("user_banned")]
    public Optional<string> UserBanned { get; set; }
}