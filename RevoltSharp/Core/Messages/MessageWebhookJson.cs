using Newtonsoft.Json;

namespace RevoltSharp;

internal class MessageWebhookJson
{
    [JsonProperty("id")]
    public string Id;

    [JsonProperty("name")]
    public string Name;

    [JsonProperty("avatar")]
    public AttachmentJson? Avatar;
}
