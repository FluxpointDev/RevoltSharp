using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events.Users;
internal class UserPlatformWipedEventJson
{
    [JsonProperty("user_id")]
    internal string UserId;
}
