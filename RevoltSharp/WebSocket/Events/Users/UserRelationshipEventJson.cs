using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events
{
    internal class UserRelationshipEventJson
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("user")]
        public string User;
    }
}
