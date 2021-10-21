using Newtonsoft.Json;
using Optional;

namespace RevoltSharp.WebSocket.Events
{
    internal class UserUpdateEventJson
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("data")]
        public PartialUserJson Data;

        [JsonProperty("clear")]
        public Option<string> Clear;
    }
}
