using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events;

internal class UserPlatformWipedEventJson
{
    [JsonProperty("user_id")]
    internal string? UserId;
}
