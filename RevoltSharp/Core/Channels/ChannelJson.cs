using Newtonsoft.Json;
using System.Collections.Generic;

namespace RevoltSharp;


internal class ChannelJson
{
    [JsonProperty("_id")]
    public string Id = null!;

    [JsonProperty("channel_type")]
    public ChannelType ChannelType;

    [JsonProperty("nonce")]
    public string? Nonce;

    [JsonProperty("user")]
    public string? UserId;

    [JsonProperty("active")]
    public bool Active;

    [JsonProperty("recipients")]
    public string[]? Recipients;

    [JsonProperty("last_message_id")]
    public string? LastMessageId;

    [JsonProperty("icon")]
    public AttachmentJson? Icon;

    [JsonProperty("description")]
    public string? Description;

    [JsonProperty("name")]
    public string? Name;

    [JsonProperty("owner")]
    public string? OwnerId;

    [JsonProperty("permissions")]
    public ulong Permissions;

    [JsonProperty("default_permissions")]
    public PermissionsJson? DefaultPermissions;

    [JsonProperty("role_permissions")]
    public Dictionary<string, PermissionsJson>? RolePermissions;

    [JsonProperty("server")]
    public string? ServerId;

    [JsonProperty("nsfw")]
    public bool IsNsfw;
}