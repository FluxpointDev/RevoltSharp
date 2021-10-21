using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events
{
    internal class ChannelDeleteEventJson
    {
        [JsonProperty("id")]
        public string Id;
    }
}
