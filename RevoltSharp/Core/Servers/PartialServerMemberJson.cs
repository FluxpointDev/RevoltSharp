using Newtonsoft.Json;
using Optional;
using System.Collections.Generic;

namespace RevoltSharp
{
    public class PartialServerMemberJson
    {
        [JsonProperty("nickname")]
        public Option<string> Nickname;

        [JsonProperty("avatar")]
        public Option<AttachmentJson> Avatar;

        [JsonProperty("roles")]
        public Option<HashSet<string>> Roles;
    }
}
