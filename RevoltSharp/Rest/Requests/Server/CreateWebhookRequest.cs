using Newtonsoft.Json;
using Optionals;

namespace RevoltSharp.Rest.Requests;

internal class CreateWebhookRequest : IRevoltRequest
{
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("avatar")]
    public Optional<string> Avatar { get; set; }
}