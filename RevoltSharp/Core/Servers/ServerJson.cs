using Newtonsoft.Json;
using System.Collections.Generic;

namespace RevoltSharp
{
    public class ServerJson
    {
        [JsonProperty("_id")]
        public string Id;

        [JsonProperty("nonce")]
        public string Nonce;

        [JsonProperty("owner")]
        public string Owner;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("channels")]
        public string[] Channels;

        [JsonProperty("categories")]
        public CategoryJson[] Categories;

        [JsonProperty("system_messages")]
        public ServerSystemMessagesJson SystemMessages;

        [JsonProperty("roles")]
        public Dictionary<string, RoleJson> Roles;

        [JsonProperty("default_permissions")]
        public ulong DefaultPermissions;

        [JsonProperty("icon")]
        public AttachmentJson Icon;

        [JsonProperty("banner")]
        public AttachmentJson Banner;
    }

    public class ServerSystemMessagesJson
    {
        [JsonProperty("user_joined")]
        public string UserJoined;

        [JsonProperty("user_left")]
        public string UserLeft;

        [JsonProperty("user_kicked")]
        public string UserKicked;

        [JsonProperty("user_banned")]
        public string UserBanned;
    }
}
