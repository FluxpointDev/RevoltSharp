using Newtonsoft.Json;

namespace RevoltSharp;


internal class MessageWebhookJson
{
    [JsonProperty("id")]
    public string Id = null!;

    [JsonProperty("name")]
    public string Name = null!;

    [JsonProperty("avatar")]
    public AttachmentJson? Avatar;
}