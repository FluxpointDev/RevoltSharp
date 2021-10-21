using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events
{
    internal class ReadyEventJson
    {
        [JsonProperty("users")]
        public UserJson[] Users;

        [JsonProperty("servers")]
        public ServerJson[] Servers;

        [JsonProperty("channels")]
        public ChannelJson[] Channels;

        [JsonProperty("members")]
        public ServerMemberJson[] Members;
    }
    internal class ServerMemberJson
    {
        [JsonProperty("server")]
        public string Server;

        [JsonProperty("user")]
        public string User;

        [JsonProperty("roles")]
        public string[] Roles;
    }
}
