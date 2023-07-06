using Newtonsoft.Json;

namespace RevoltSharp;


internal class CreateInviteJson
{
    [JsonProperty("_id")]
    public string Code = null!;

    [JsonProperty("creator")]
    public string CreatorId = null!;

    [JsonProperty("channel")]
    public string ChannelId = null!;

    [JsonProperty("type")]
    public string ChannelType = null!;
}
internal class InviteJson
{
    [JsonProperty("code")]
    public string Code = null!;

    [JsonProperty("channel_id")]
    public string ChannelId = null!;

    [JsonProperty("channel_name")]
    public string ChannelName = null!;

    [JsonProperty("channel_description")]
    public string? ChannelDescription;

    [JsonProperty("user_name")]
    public string CreatorName = null!;

    [JsonProperty("user_avatar")]
    public AttachmentJson? CreatorAvatar;

    [JsonProperty("type")]
    public string? ChannelType;
}