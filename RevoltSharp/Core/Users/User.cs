using Optional.Unsafe;
using System;

namespace RevoltSharp
{
    public class User
    {
        public string Id { get; internal set; }
        public string Username { get; internal set; }

        public string Status { get; internal set; }

        public string ProfileBio { get; internal set; }

        public Attachment Avatar { get; internal set; }

        public UserBadges Badges { get; internal set; }

        public BotData BotData { get; internal set; }

        public bool IsOnline { get; internal set; }

        public string Relationship { get; internal set; }

        public bool IsBot
            => BotData != null;
        public bool HasBadge(UserBadgeTypes type)
            => Badges.Types.HasFlag(type);

        internal RevoltClient Client;

        internal static User Create(RevoltClient client, UserJson json)
        {
            return new User
            {
                Id = json.id,
                Username = json.username,
                BotData = json.bot != null ? new BotData { Owner = json.bot.owner } : null,
                Avatar = json.avatar != null ? json.avatar.ToEntity() : null,
                Badges = new UserBadges { Raw = json.badges },
                IsOnline = json.online,
                ProfileBio = json.profile != null ? json.profile.content : null,
                Relationship = json.relationship,
                Status = json.status != null ? json.status.text : null,
                Client = client
            };
        }

        internal void Update(PartialUserJson data)
        {
            if (data.avatar.HasValue)
                Avatar = data.avatar.ValueOrDefault().ToEntity();
            if (data.status.HasValue)
                Status = data.status.ValueOrDefault().text;
        }
    }
    public class UserBadges
    {
        public int Raw { get; internal set; }
        public UserBadgeTypes Types
            => (UserBadgeTypes)Raw;
    }
    public class BotData
    {
        public string Owner { get; internal set; }
    }
    [Flags]
    public enum UserBadgeTypes
    {
        Developer = 1,
        Translator = 2,
        Supporter = 4,
        ResponsibleDisclosure = 8,
        EarlyAdopter = 256
    }
    public enum UserStatusType
    {
        Online, Idle, Busy, Invisible
    }
}
