using Newtonsoft.Json;

namespace RevoltSharp
{
    internal class ServerMemberJson
    {
        [JsonProperty("_id")]
        public ServerMemberIdsJson id;
        public string nickname;
        public AttachmentJson avatar;
        public string[] roles;
    }
    internal class ServerMemberIdsJson
    {
        public string server;
        public string user;
    }
}
