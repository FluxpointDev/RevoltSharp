using Newtonsoft.Json;

namespace RevoltSharp
{
    public class ProfileJson
    {
        [JsonProperty("content")]
        public string Content;

        [JsonProperty("background")]
        public AttachmentJson Background;
    }
}
