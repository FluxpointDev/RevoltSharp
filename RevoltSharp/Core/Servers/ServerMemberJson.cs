using Newtonsoft.Json;

namespace RevoltSharp
{
    internal class ServerMemberJson
    {
        [JsonProperty("_id")]
        public ServerMemberIdsJson Id;

        [JsonProperty("nickname")]
        public string Nickname;

        [JsonProperty("avatar")]
        public AttachmentJson Avatar;

        [JsonProperty("roles")]
        public string[] Roles;
    }
    internal class ServerMemberIdsJson
    {
        [JsonProperty("server")]
        public string Server;

        [JsonProperty("user")]
        public string User;
    }
}
