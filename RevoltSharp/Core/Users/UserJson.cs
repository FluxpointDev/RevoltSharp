using Newtonsoft.Json;

namespace RevoltSharp;

internal class UserJson
{
    [JsonProperty("_id")]
    public string? Id;

    [JsonProperty("username")]
    public string? Username;

    [JsonProperty("display_name")]
    public string? DisplayName;

    [JsonProperty("discriminator")]
    public string? Discriminator;

    [JsonProperty("avatar")]
    public AttachmentJson? Avatar;

    [JsonProperty("badges")]
    public ulong Badges;

    [JsonProperty("status")]
    public UserStatusJson? Status;

    [JsonProperty("profile")]
    public UserProfileJson? Profile;

    [JsonProperty("bot")]
    public UserBotJson? Bot;

    [JsonProperty("relationship")]
    public string? Relationship;

    [JsonProperty("online")]
    public bool Online;

    [JsonProperty("privileged")]
    public bool Privileged;

    [JsonProperty("flags")]
    public ulong Flags;
}
internal class UserStatusJson
{
    [JsonProperty("text")]
    public string? Text;

    [JsonProperty("presence")]
    public string? Presence;
}
internal class UserBotJson
{
    [JsonProperty("owner")]
    public string? Owner;
}
internal class UserProfileJson
{
    [JsonProperty("content")]
    public string? Content;

    [JsonProperty("background")]
    public AttachmentJson? Background;
}
