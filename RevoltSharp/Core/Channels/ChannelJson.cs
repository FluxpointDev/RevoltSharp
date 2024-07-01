using Newtonsoft.Json;
using System.Collections.Generic;

namespace RevoltSharp;


internal class ChannelJson
{
    [JsonProperty("_id")]
    public string Id { get; set; }

    [JsonProperty("channel_type")]
    public ChannelType ChannelType { get; set; }

    [JsonProperty("nonce")]
    public string? Nonce { get; set; }

    [JsonProperty("user")]
    public string? UserId { get; set; }

    [JsonProperty("active")]
    public bool Active { get; set; }

    [JsonProperty("recipients")]
    public string[]? Recipients { get; set; }

    [JsonProperty("last_message_id")]
    public string? LastMessageId { get; set; }

    [JsonProperty("icon")]
    public AttachmentJson? Icon { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("owner")]
    public string? OwnerId { get; set; }

    [JsonProperty("permissions")]
    public ulong Permissions { get; set; }

    [JsonProperty("default_permissions")]
    public PermissionsJson? DefaultPermissions { get; set; }

    [JsonProperty("role_permissions")]
    public Dictionary<string, PermissionsJson>? RolePermissions { get; set; }

    [JsonProperty("server")]
    public string? ServerId { get; set; }

    [JsonProperty("nsfw")]
    public bool IsNsfw { get; set; }
}