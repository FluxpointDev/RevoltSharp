using Newtonsoft.Json;
using Optional;

namespace RevoltSharp.WebSocket.Events
{
    internal class ServerMemberUpdateEventJson
    {
        [JsonProperty("id")]
        public ServerMemberIdsJson Id;

        [JsonProperty("data")]
        public PartialServerMemberJson Data;

        [JsonProperty("clear")]
        public Option<string> Clear;
    }
}
