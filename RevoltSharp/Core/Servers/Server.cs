using Optional.Unsafe;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RevoltSharp
{
    public class Server
    {
        public string Id { get; internal set; }
        public string OwnerId { get; internal set; }
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public HashSet<string> ChannelIds { get; internal set; }
        //public ServerCategory[] Categories;
        //public ServerSystemMessages SystemMessages;
        public ConcurrentDictionary<string, Role> Roles { get; internal set; }
        public int[] DefaultPermissions { get; internal set; }
        public Attachment Icon { get; internal set; }
        public Attachment Banner { get; internal set; }
        internal RevoltClient Client;

        public Role GetRole(string roleId)
        {
            if (Roles.TryGetValue(roleId, out Role Role))
                return Role;
            return null;
        }

        public TextChannel GetTextChannel(string channelId)
        {
            if (Client.WebSocket.ChannelCache.TryGetValue(channelId, out Channel Chan))
            {
                if (Chan.ServerId != Id)
                    return null;
                return Chan as TextChannel;
            }
            return null;
        }

        public VoiceChannel GetVoiceChannel(string channelId)
        {
            if (Client.WebSocket.ChannelCache.TryGetValue(channelId, out Channel Chan))
            {
                if (Chan.ServerId != Id)
                    return null;
                return Chan as VoiceChannel;
            }
            return null;
        }

        internal void Update(PartialServerJson json)
        {
            if (json.name.HasValue)
                Name = json.name.ValueOrDefault();

            if (json.icon.HasValue)
                Icon = json.icon.ValueOrDefault() != null ? json.icon.ValueOrDefault().ToEntity() : null;

            if (json.banner.HasValue)
                Banner = json.banner.ValueOrDefault() != null ? json.banner.ValueOrDefault().ToEntity() : null;

            if (json.default_permissions.HasValue)
                DefaultPermissions = json.default_permissions.ValueOrDefault();

            if (json.description.HasValue)
                Description = json.description.ValueOrDefault();
        }

        internal Server Clone()
        {
            return (Server)this.MemberwiseClone();
        }
    }
}
