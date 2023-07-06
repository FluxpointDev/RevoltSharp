using Newtonsoft.Json;

namespace RevoltSharp;


internal class WebhookJson
{
    [JsonProperty("id")]
    public string? Id;

    [JsonProperty("name")]
    public string? Name;

    [JsonProperty("avatar")]
    public AttachmentJson? Avatar;

    [JsonProperty("channel")]
    public string? ChannelId;

    [JsonProperty("permissions")]
    public ulong? Permissions;

    [JsonProperty("token")]
    public string? Token;
}