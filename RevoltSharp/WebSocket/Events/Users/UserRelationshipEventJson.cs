using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events;

internal class UserRelationshipEventJson
{
    [JsonProperty("id")]
    public string Id;

    [JsonProperty("user")]
    public UserJson User;

    [JsonProperty("status")]
    public string Status;
}
