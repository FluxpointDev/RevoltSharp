using Newtonsoft.Json;

namespace RevoltSharp;


internal class EmojiJson
{
    [JsonProperty("_id")]
    public string Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("parent")]
    public EmojiParentJson Parent { get; set; }

    [JsonProperty("creator_id")]
    public string CreatorId { get; set; }

    [JsonProperty("animated")]
    public bool Animated { get; set; }

    [JsonProperty("nsfw")]
    public bool Nsfw { get; set; }
}
internal class EmojiParentJson
{
    [JsonProperty("id")]
    public string ServerId { get; set; }
}