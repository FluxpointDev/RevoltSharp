using Newtonsoft.Json;
using Optional;
using System.Collections.Generic;

namespace RevoltSharp
{
    internal class PartialChannelJson
    {
        [JsonProperty("name")]
        public Option<string> Name { get; set; }

        [JsonProperty("icon")]
        public Option<AttachmentJson> Icon { get; set; }

        [JsonProperty("description")]
        public Option<string> Description { get; set; }

        [JsonProperty("deafult_permissions")]
        public Option<ulong> DefaultPermissions { get; set; }

        [JsonProperty("role_permissions")]
        public Option<Dictionary<string, ulong>> RolePermissions { get; set; }
    }
}
