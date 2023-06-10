using Newtonsoft.Json;

namespace RevoltSharp;

internal class MembersListJson
{
    [JsonProperty("members")]
    public ServerMemberJson[]? Members;

    [JsonProperty("users")]
    public UserJson[]? Users;
}
