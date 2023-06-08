using Newtonsoft.Json;

namespace RevoltSharp;

internal class AttachmentJson
{
    [JsonProperty("_id")]
    public string Id { get; set; }

    [JsonProperty("tag")]
    public string Tag { get; set; }

    [JsonProperty("filename")]
    public string Filename { get; set; }

    [JsonProperty("metadata")]
    public AttachmentMetaJson Metadata { get; set; }

    [JsonProperty("content_type")]
    public string ContentType { get; set; }

    [JsonProperty("size")]
    public int Size { get; set; }

    [JsonProperty("deleted")]
    public bool Deleted { get; set; }

    [JsonProperty("reported")]
    public bool Reported { get; set; }
}
internal class AttachmentMetaJson
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("width")]
    public int Width { get; set; }

    [JsonProperty("height")]
    public int Height { get; set; }
}
