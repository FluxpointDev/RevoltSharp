using Newtonsoft.Json;

namespace RevoltSharp;


internal class UserJson
{
    [JsonProperty("_id")]
    public string Id { get; set; } = null!;

    [JsonProperty("username")]
    public string Username { get; set; } = null!;

    [JsonProperty("display_name")]
    public string? DisplayName { get; set; }

    [JsonProperty("discriminator")]
    public string Discriminator { get; set; } = null!;

    [JsonProperty("avatar")]
    public AttachmentJson? Avatar { get; set; }

    [JsonProperty("badges")]
    public ulong Badges { get; set; }

    [JsonProperty("status")]
    public UserStatusJson? Status { get; set; }

    [JsonProperty("profile")]
    public UserProfileJson? Profile { get; set; }

    [JsonProperty("bot")]
    public UserBotJson? Bot { get; set; }

    [JsonProperty("relationship")]
    public string? Relationship { get; set; }

    [JsonProperty("online")]
    public bool Online { get; set; }

    [JsonProperty("privileged")]
    public bool Privileged { get; set; }

    [JsonProperty("flags")]
    public ulong Flags { get; set; }
}
internal class UserStatusJson
{
    [JsonProperty("text")]
    public string? Text { get; set; }

    [JsonProperty("presence")]
    public string? Presence { get; set; }
}
internal class UserBotJson
{
    [JsonProperty("owner")]
    public string Owner { get; set; } = null!;
}
internal class UserProfileJson
{
    [JsonProperty("content")]
    public string? Content { get; set; }

    [JsonProperty("background")]
    public AttachmentJson? Background { get; set; }
}