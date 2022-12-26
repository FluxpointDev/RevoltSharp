using Newtonsoft.Json;
using Optionals;

namespace RevoltSharp
{
    internal class PartialServerJson
    {
        [JsonProperty("name")]
        public Optional<string> Name { get; set; }

        [JsonProperty("icon")]
        public Optional<AttachmentJson> Icon { get; set; }

        [JsonProperty("banner")]
        public Optional<AttachmentJson> Banner { get; set; }

        [JsonProperty("description")]
        public Optional<string> Description { get; set; }

        [JsonProperty("default_permissions")]
        public Optional<ulong> DefaultPermissions { get; set; }

        [JsonProperty("analytics")]
        public Optional<bool> Analytics;

        [JsonProperty("discoverable")]
        public Optional<bool> Discoverable;

        [JsonProperty("nsfw")]
        public Optional<bool> Nsfw;

        [JsonProperty("owner")]
        public Optional<string> Owner;
    }
}
