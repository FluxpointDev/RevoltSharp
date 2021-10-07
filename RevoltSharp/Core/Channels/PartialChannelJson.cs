using Newtonsoft.Json;
using Optional;

namespace RevoltSharp
{
    internal class PartialChannelJson
    {
        [JsonProperty("name")]
        public Option<string> Name { get; set; }

        [JsonProperty("icon")]
        public Option<AttachmentJson> Icon { get; set; }

        [JsonProperty("description")]
        public Option<string> Description { get; set; }

        [JsonProperty("deafult_permissions")]
        public Option<int> DefaultPermissions { get; set; }
    }
}
