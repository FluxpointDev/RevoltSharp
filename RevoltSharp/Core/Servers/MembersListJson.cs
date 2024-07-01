using Newtonsoft.Json;

namespace RevoltSharp;


internal class MembersListJson
{
    [JsonProperty("members")]
    public ServerMemberJson[]? Members { get; set; }

    [JsonProperty("users")]
    public UserJson[]? Users { get; set; }
}
