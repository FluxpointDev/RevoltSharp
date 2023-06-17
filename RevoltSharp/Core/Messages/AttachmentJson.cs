using Newtonsoft.Json;

namespace RevoltSharp;

internal class AttachmentJson
{
    [JsonProperty("_id")]
    public string Id { get; set; } = null!;

    [JsonProperty("tag")]
    public string Tag { get; set; } = null!;

    [JsonProperty("filename")]
    public string Filename { get; set; } = null!;

    [JsonProperty("metadata")]
    public AttachmentMetaJson Metadata { get; set; } = null!;

    [JsonProperty("content_type")]
    public string ContentType { get; set; } = null!;

    [JsonProperty("size")]
    public int Size { get; set; }

    [JsonProperty("deleted")]
    public bool? Deleted { get; set; }

    [JsonProperty("reported")]
    public bool? Reported { get; set; }
}
internal class AttachmentMetaJson
{
    [JsonProperty("type")]
    public string Type { get; set; } = null!;

    [JsonProperty("width")]
    public int? Width { get; set; }

    [JsonProperty("height")]
    public int? Height { get; set; }
}
