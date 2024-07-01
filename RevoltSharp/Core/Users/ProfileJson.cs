using Newtonsoft.Json;
using Optionals;

namespace RevoltSharp;


internal class ProfileJson
{
    [JsonProperty("content")]
    public Optional<string?> Content { get; set; }

    [JsonProperty("background")]
    public Optional<AttachmentJson?> Background { get; set; }
}