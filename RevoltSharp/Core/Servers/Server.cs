using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RevoltSharp
{
    public class Server : Entity
    {
        internal Server(RevoltClient client, ServerJson model) : base(client)
        {
            Id = model.Id;
                Name = model.Name;
            DefaultPermissions = new ServerPermissions(model.DefaultPermissions);
            Description = model.Description;
            Banner = model.Banner != null ? new Attachment(model.Banner) : null;
            Icon = model.Icon != null ? new Attachment(model.Icon) : null;
            ChannelIds = model.Channels != null ? model.Channels.ToHashSet() : new HashSet<string>();
            OwnerId = model.Owner;
            InternalRoles = model.Roles != null
                ? new ConcurrentDictionary<string, Role>(model.Roles.ToDictionary(x => x.Key, x => new Role(client, x.Value, model.Id, x.Key)))
                : new ConcurrentDictionary<string, Role>();
            HasAnalytics = model.Analytics;
            IsDiscoverable = model.Discoverable;
            IsNsfw = model.Nsfw;
            if (model.SystemMessages == null)
                SystemMessages = new ServerSystemMessages();
            else
            {
                SystemMessages = new ServerSystemMessages
                {
                    UserBanned = model.SystemMessages.UserBanned,
                    UserJoined = model.SystemMessages.UserJoined,
                    UserKicked = model.SystemMessages.UserKicked,
                    UserLeft = model.SystemMessages.UserLeft
                };
            }
        }

        public string Id { get; internal set; }

        public string OwnerId { get; internal set; }

        public string Name { get; internal set; }

        public string Description { get; internal set; }

        internal HashSet<string> ChannelIds { get; set; }

        //public ServerCategory[] Categories;
        public ServerSystemMessages SystemMessages;

        internal ConcurrentDictionary<string, Role> InternalRoles { get; set; }

        internal ConcurrentDictionary<string, Emoji> InternalEmojis { get; set; } = new ConcurrentDictionary<string, Emoji>();

        internal ConcurrentDictionary<string, ServerMember> InternalMembers { get; set; } = new ConcurrentDictionary<string, ServerMember>();

        public ServerPermissions DefaultPermissions { get; internal set; }

        public Attachment Icon { get; internal set; }

        public Attachment Banner { get; internal set; }

        public bool HasAnalytics { get; internal set; }

        public bool IsDiscoverable { get; internal set; }

        public bool IsNsfw { get; internal set; }

        public ServerMember GetCachedMember(string userId)
        {
            if (InternalMembers.TryGetValue(userId, out ServerMember member))
                return member;
            return null;
        }

        [JsonIgnore]
        public IReadOnlyCollection<ServerMember> CachedMembers
            => (IReadOnlyCollection<ServerMember>)InternalMembers.Values;

        public Role GetRole(string roleId)
        {
            if (InternalRoles.TryGetValue(roleId, out Role role))
                return role;
            return null;
        }

        [JsonIgnore]
        public IReadOnlyCollection<Role> Roles
            => (IReadOnlyCollection<Role>)InternalRoles.Values;

        public Emoji GetEmoji(string emojiId)
        {
            if (InternalEmojis.TryGetValue(emojiId, out Emoji emoji))
                return emoji;
            return null;
        }

        [JsonIgnore]
        public IReadOnlyCollection<Emoji> Emojis
            => (IReadOnlyCollection<Emoji>)InternalEmojis.Values;

        public TextChannel GetTextChannel(string channelId)
        {
            if (!Client.WebSocket.ChannelCache.TryGetValue(channelId, out Channel chan))
                return null;

            if (chan is not TextChannel textChannel)
                return null;

            if (textChannel.ServerId != Id)
                return null;

            return textChannel;
        }

        public VoiceChannel GetVoiceChannel(string channelId)
        {
            if (!Client.WebSocket.ChannelCache.TryGetValue(channelId, out Channel chan))
                return null;

            if (chan is not VoiceChannel voiceChannel)
                return null;

            if (voiceChannel.ServerId != Id)
                return null;

            return voiceChannel;
        }

        internal void Update(PartialServerJson json)
        {
            if (json.Name.HasValue)
                Name = json.Name.Value;

            if (json.Icon.HasValue)
                Icon = new Attachment(json.Icon.Value);

            if (json.Banner.HasValue)
                Banner = new Attachment(json.Icon.Value);

            if (json.DefaultPermissions.HasValue)
                DefaultPermissions = new ServerPermissions(json.DefaultPermissions.Value);

            if (json.Description.HasValue)
                Description = json.Description.Value;

            if (json.Analytics.HasValue)
                HasAnalytics = json.Analytics.Value;

            if (json.Discoverable.HasValue)
                IsDiscoverable = json.Discoverable.Value;

            if (json.Nsfw.HasValue)
                IsNsfw = json.Nsfw.Value;

            if (json.Owner.HasValue)
                OwnerId = json.Owner.Value;

            if (json.SystemMessages.HasValue)
            {
                SystemMessages = new ServerSystemMessages
                {
                    UserBanned = json.SystemMessages.Value.UserBanned,
                    UserJoined = json.SystemMessages.Value.UserJoined,
                    UserKicked = json.SystemMessages.Value.UserKicked,
                    UserLeft = json.SystemMessages.Value.UserLeft
                };
            }
        }

        internal Server Clone()
        {
            return (Server) this.MemberwiseClone();
        }

        internal void AddMember(ServerMember member)
        {
            InternalMembers.TryAdd(member.Id, member);
            member.User.InternalMutualServers.TryAdd(Id, this);
        }

        internal void RemoveMember(User user, bool delete)
        {
            if (!delete)
                InternalMembers.TryRemove(user.Id, out _);

            user.InternalMutualServers.TryRemove(Id, out _);
            if (user.Id != user.Client.CurrentUser.Id && !user.HasMutuals())
            {
                user.Client.WebSocket.UserCache.TryRemove(user.Id, out _);
            }
        }
    }
}
