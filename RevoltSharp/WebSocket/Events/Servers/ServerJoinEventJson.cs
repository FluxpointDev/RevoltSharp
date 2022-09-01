using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events
{
    internal class ServerJoinEventJson
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("server")]
        public Server Server;

        [JsonProperty("channels")]
        public ServerChannel[] Channels;
    }
}
