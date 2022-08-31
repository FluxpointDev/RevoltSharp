using Newtonsoft.Json;

namespace RevoltSharp
{
    internal class ProfileJson
    {
        [JsonProperty("content")]
        public string Content;

        [JsonProperty("background")]
        public AttachmentJson Background;
    }
}
