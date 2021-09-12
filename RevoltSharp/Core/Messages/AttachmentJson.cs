using Newtonsoft.Json;

namespace RevoltSharp
{
    internal class AttachmentJson
    {
        [JsonProperty("_id")]
        public string id;
        public string tag;
        public string filename;
        public AttachmentMetaJson metadata;
        public string content_type;
        public int size;

        public Attachment ToEntity()
        {
            return new Attachment
            {
                Filename = filename,
                Id = id,
                Height = metadata.height,
                Size = size,
                Tag = tag,
                Type = content_type,
                Width = metadata.width
            };
        }
    }
    internal class AttachmentMetaJson
    {
        public string type;
        public int width;
        public int height;
    }
}
