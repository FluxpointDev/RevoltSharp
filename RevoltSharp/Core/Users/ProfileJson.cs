using Newtonsoft.Json;
using Optionals;

namespace RevoltSharp;


internal class ProfileJson
{
    [JsonProperty("content")]
    public Optional<string?> Content;

    [JsonProperty("background")]
    public Optional<AttachmentJson?> Background;
}