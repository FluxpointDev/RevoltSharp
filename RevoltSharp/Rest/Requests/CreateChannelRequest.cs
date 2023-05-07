using Newtonsoft.Json;
using Optionals;

namespace RevoltSharp.Rest.Requests;

internal class CreateChannelRequest : RevoltRequest
{
    public string name;
    [JsonProperty("type")]
    public string Type;
    public Optional<string> description;
    public Optional<string[]> users;
    public Optional<bool> nsfw;

}
