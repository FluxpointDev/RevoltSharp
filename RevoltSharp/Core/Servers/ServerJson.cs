using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RevoltSharp
{
    internal class ServerJson
    {
        [JsonProperty("_id")]
        public string id;
        public string nonce;
        public string owner;
        public string name;
        public string description;
        public string[] channels;
        public CategoryJson[] categories;
        public ServerSystemMessagesJson system_messages;
        public Dictionary<string, RoleJson> roles;
        public int[] default_permissions;
        public AttachmentJson icon;
        public AttachmentJson banner;

        internal Server ToEntity(RevoltClient client)
        {
            return new Server
            {
                Id = id,
                Name = name,
                DefaultPermissions = default_permissions,
                Description = description,
                Banner = banner != null ? banner.ToEntity() : null,
                ChannelIds = channels != null ? channels.ToHashSet() : new HashSet<string>(),
                Icon = icon != null ? icon.ToEntity() : null,
                OwnerId = owner,
                Roles = roles != null ? new ConcurrentDictionary<string, Role>(roles.ToDictionary(x => x.Key, x => x.Value.ToEntity(client, id, x.Key))) : new ConcurrentDictionary<string, Role>(),
                Client = client
            };
        }
    }

    internal class ServerSystemMessagesJson
    {
        public string user_joined;
        public string user_left;
        public string user_kicked;
        public string user_banned;
    }
}
