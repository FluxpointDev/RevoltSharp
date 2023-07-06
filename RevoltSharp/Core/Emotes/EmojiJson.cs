using Newtonsoft.Json;

namespace RevoltSharp;


internal class EmojiJson
{
    [JsonProperty("_id")]
    public string Id = null!;

    [JsonProperty("name")]
    public string Name = null!;

    [JsonProperty("parent")]
    public EmojiParentJson Parent = null!;

    [JsonProperty("creator_id")]
    public string CreatorId = null!;

    [JsonProperty("animated")]
    public bool Animated;

    [JsonProperty("nsfw")]
    public bool Nsfw;
}
internal class EmojiParentJson
{
    [JsonProperty("id")]
    public string ServerId = null!;
}