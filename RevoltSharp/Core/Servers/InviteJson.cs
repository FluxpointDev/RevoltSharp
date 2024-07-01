using Newtonsoft.Json;

namespace RevoltSharp;


internal class CreateInviteJson
{
    [JsonProperty("_id")]
    public string Code { get; set; } = null!;

    [JsonProperty("creator")]
    public string CreatorId { get; set; } = null!;

    [JsonProperty("channel")]
    public string ChannelId { get; set; } = null!;

    [JsonProperty("type")]
    public string ChannelType { get; set; } = null!;
}
internal class InviteJson
{
    [JsonProperty("code")]
    public string Code { get; set; } = null!;

    [JsonProperty("channel_id")]
    public string ChannelId { get; set; } = null!;

    [JsonProperty("channel_name")]
    public string ChannelName { get; set; } = null!;

    [JsonProperty("channel_description")]
    public string? ChannelDescription { get; set; }

    [JsonProperty("user_name")]
    public string CreatorName { get; set; } = null!;

    [JsonProperty("user_avatar")]
    public AttachmentJson? CreatorAvatar { get; set; }

    [JsonProperty("type")]
    public string? ChannelType { get; set; }
}