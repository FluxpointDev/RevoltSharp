using Newtonsoft.Json;
using Optional;

namespace RevoltSharp
{
    internal class PartialServerJson
    {
        [JsonProperty("name")]
        public Option<string> Name { get; set; }

        [JsonProperty("icon")]
        public Option<AttachmentJson> Icon { get; set; }

        [JsonProperty("banner")]
        public Option<AttachmentJson> Banner { get; set; }

        [JsonProperty("description")]
        public Option<string> Description { get; set; }

        [JsonProperty("default_permissions")]
        public Option<ulong> DefaultPermissions { get; set; }
    }
}
