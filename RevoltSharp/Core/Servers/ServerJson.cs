using Newtonsoft.Json;
using Optionals;
using System.Collections.Generic;

namespace RevoltSharp
{
    internal class ServerJson
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

        [JsonProperty("analytics")]
        public bool Analytics;

        [JsonProperty("discoverable")]
        public bool Discoverable;

        [JsonProperty("nsfw")]
        public bool Nsfw;
    }

    internal class ServerSystemMessagesJson
    {
        [JsonProperty("user_joined")]
        public Optional<string> UserJoined;

        [JsonProperty("user_left")]
        public Optional<string> UserLeft;

        [JsonProperty("user_kicked")]
        public Optional<string> UserKicked;

        [JsonProperty("user_banned")]
        public Optional<string> UserBanned;
    }
}
