using Newtonsoft.Json;
using Optionals;

namespace RevoltSharp.WebSocket.Events
{
    internal class ServerMemberUpdateEventJson
    {
        [JsonProperty("id")]
        public ServerMemberIdsJson Id;

        [JsonProperty("data")]
        public PartialServerMemberJson Data;

        [JsonProperty("clear")]
        public Optional<string[]> Clear;
    }
}
