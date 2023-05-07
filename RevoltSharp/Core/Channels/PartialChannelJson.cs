using Newtonsoft.Json;
using Optionals;
using System.Collections.Generic;

namespace RevoltSharp
{
    internal class PartialChannelJson
    {
        [JsonProperty("name")]
        public Optional<string> Name { get; set; }

        [JsonProperty("icon")]
        public Optional<AttachmentJson?> Icon { get; set; }

        [JsonProperty("description")]
        public Optional<string> Description { get; set; }

        [JsonProperty("deafult_permissions")]
        public Optional<PermissionsJson> DefaultPermissions { get; set; }

        [JsonProperty("role_permissions")]
        public Optional<Dictionary<string, PermissionsJson>> RolePermissions { get; set; }

        [JsonProperty("nsfw")]
        public Optional<bool> IsNsfw { get; set; }

        [JsonProperty("owner")]
        public Optional<string> OwnerId { get; set; }
    }
}
