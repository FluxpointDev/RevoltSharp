using Optional.Unsafe;
using System;

namespace RevoltSharp
{
    public class User : Entity
    {
        public string Id { get; }

        public string Username { get; }

        public string Status { get; internal set; }

        public Attachment Avatar { get; internal set; }

        public UserBadges Badges { get; }

        public BotData BotData { get; }

        public bool IsOnline { get; }

        public string Relationship { get; }

        public bool IsBot => BotData != null;

        internal UserJson Model { get; }

        public User(RevoltClient client, UserJson model)
            : base(client)
        {
            Model = model;
            Id = model.Id;
            Username = model.Username;
            BotData = model.Bot != null ? new BotData { Owner = model.Bot.Owner } : null;
            Avatar = model.Avatar != null ? new Attachment(client, model.Avatar) : null;
            Badges = new UserBadges { Raw = model.Badges };
            IsOnline = model.Online;
            Relationship = model.Relationship;
            Status = model.Status?.Text;
        }
        public bool HasBadge(UserBadgeTypes type)
            => Badges.Types.HasFlag(type);

        internal void Update(PartialUserJson data)
        {
            if (data.avatar.HasValue)
                Avatar = data.avatar.ValueOrDefault() != null ? new Attachment(Client, data.avatar.ValueOrDefault()) : null;
            if (data.status.HasValue)
                Status = data.status.ValueOrDefault() != null ? data.status.ValueOrDefault().Text : null;
            if (this is SelfUser Self)
            {
                if (data.ProfileBackground.HasValue)
                    Self.Background = data.ProfileBackground.ValueOrDefault() != null ? new Attachment(Client, data.ProfileBackground.ValueOrDefault()) : null;

                if (data.ProfileContent.HasValue)
                    Self.ProfileBio = data.ProfileContent.ValueOrDefault();
            }
        }

        internal User Clone()
        {
            return (User)this.MemberwiseClone();
        }
    }
    public class UserBadges
    {
        public int Raw { get; internal set; }

        public UserBadgeTypes Types
            => (UserBadgeTypes) Raw;
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
