using Newtonsoft.Json;
using Optionals;

namespace RevoltSharp.WebSocket.Events
{
    internal class UserUpdateEventJson
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("data")]
        public PartialUserJson Data;

        [JsonProperty("clear")]
        public Optional<string[]> Clear;
    }
}
