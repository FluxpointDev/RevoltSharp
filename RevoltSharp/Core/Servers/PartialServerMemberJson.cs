using Newtonsoft.Json;
using Optionals;
using System;

namespace RevoltSharp
{
    internal class PartialServerMemberJson
    {
        [JsonProperty("nickname")]
        public Optional<string> Nickname;

        [JsonProperty("avatar")]
        public Optional<AttachmentJson> Avatar;

        [JsonProperty("roles")]
        public Optional<string[]> Roles;

        [JsonProperty("timeout")]
        public Optional<DateTime> Timeout;

        public bool ClearTimeout;
    }
}
