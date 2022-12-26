using Newtonsoft.Json;

namespace RevoltSharp
{
    internal class AttachmentJson
    {
        [JsonProperty("_id")]
        public string Id;

        [JsonProperty("tag")]
        public string Tag;

        [JsonProperty("filename")]
        public string Filename;

        [JsonProperty("metadata")]
        public AttachmentMetaJson Metadata;

        [JsonProperty("content_type")]
        public string ContentType;

        [JsonProperty("size")]
        public int Size;


    }
    internal class AttachmentMetaJson
    {
        [JsonProperty("type")]
        public string Type;

        [JsonProperty("width")]
        public int Width;

        [JsonProperty("height")]
        public int Height;
    }
}
