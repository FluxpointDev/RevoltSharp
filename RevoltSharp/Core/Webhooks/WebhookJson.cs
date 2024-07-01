using Newtonsoft.Json;

namespace RevoltSharp;


internal class WebhookJson
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("avatar")]
    public AttachmentJson? Avatar { get; set; }

    [JsonProperty("channel_id")]
    public string? ChannelId { get; set; }

    [JsonProperty("permissions")]
    public ulong? Permissions { get; set; }

    [JsonProperty("token")]
    public string? Token { get; set; }
}