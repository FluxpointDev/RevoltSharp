using Newtonsoft.Json;

namespace RevoltSharp;

internal class MessageReplyJson
{
    [JsonProperty("id")]
    public string? messageId { get; set; }

    [JsonProperty("mention")]
    public bool isMention { get; set; }
}
