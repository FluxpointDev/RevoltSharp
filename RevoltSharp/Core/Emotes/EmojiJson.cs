using Newtonsoft.Json;

namespace RevoltSharp;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
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
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.