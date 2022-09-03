using Newtonsoft.Json;
using Optional;
using System;
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

        [JsonProperty("timeout")]
        public Option<DateTime> Timeout;

        public bool ClearTimeout;
    }
}
