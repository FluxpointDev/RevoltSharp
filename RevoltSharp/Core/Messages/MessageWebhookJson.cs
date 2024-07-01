using Newtonsoft.Json;

namespace RevoltSharp;


internal class MessageWebhookJson
{
    [JsonProperty("id")]
    public string Id { get; set; } = null!;

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("avatar")]
    public AttachmentJson? Avatar { get; set; }
}