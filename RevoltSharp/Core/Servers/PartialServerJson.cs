using Newtonsoft.Json;
using Optionals;

namespace RevoltSharp;


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
    public Optional<bool> Analytics { get; set; }

    [JsonProperty("discoverable")]
    public Optional<bool> Discoverable { get; set; }

    [JsonProperty("nsfw")]
    public Optional<bool> Nsfw { get; set; }

    [JsonProperty("owner")]
    public Optional<string> Owner { get; set; }

    [JsonProperty("system_messages")]
    public Optional<ServerSystemMessagesJson> SystemMessages { get; set; }

    [JsonProperty("categories")]
    public Optional<CategoryJson[]> Categories { get; set; }
}