using Newtonsoft.Json;
using Optional;
using System.Collections.Generic;

namespace RevoltSharp
{
    internal class PartialServerMemberJson
    {
        [JsonProperty("nickname")]
        public Option<string> Nickname;

        [JsonProperty("avatar")]
        public Option<AttachmentJson> Avatar;

        [JsonProperty("roles")]
        public Option<string[]> Roles;
    }
}
