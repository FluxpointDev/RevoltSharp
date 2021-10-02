using Newtonsoft.Json;

namespace RevoltSharp
{
    public class MembersListJson
    {
        [JsonProperty("members")]
        public ServerMemberJson[] Members;

        [JsonProperty("users")]
        public UserJson[] Users;
    }
}
