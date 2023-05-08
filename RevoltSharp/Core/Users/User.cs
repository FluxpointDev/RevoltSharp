
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RevoltSharp;

public class User : CreatedEntity
{
    public string Id { get; }

    public string Username { get; internal set; }

    public UserStatus Status { get; internal set; }

    public Attachment? Avatar { get; internal set; }

    public UserBadges Badges { get; }

    public UserFlags Flags { get; internal set; }

    public BotData? BotData { get; }

    public bool IsOnline => (Status.Type != UserStatusType.Offline && Status.Type != UserStatusType.Invisible);

    public bool Privileged { get; internal set; }

    public string Relationship { get; }

    public bool IsBot => BotData != null;

    [JsonIgnore]
    internal UserJson Model { get; }

    internal ConcurrentDictionary<string, Server> InternalMutualServers { get; set; } = new ConcurrentDictionary<string, Server>();

    [JsonIgnore]
    public IReadOnlyCollection<Server> MutualServers
        => (IReadOnlyCollection<Server>)InternalMutualServers.Values;

    internal ConcurrentDictionary<string, GroupChannel> InternalMutualGroups { get; set; } = new ConcurrentDictionary<string, GroupChannel>();

    [JsonIgnore]
    public IReadOnlyCollection<GroupChannel> MutualGroups
       => (IReadOnlyCollection<GroupChannel>)InternalMutualGroups.Values;

    internal bool HasMutuals()
       => InternalMutualGroups.Any() || InternalMutualServers.Any();

    internal User(RevoltClient client, UserJson model)
        : base(client, model.Id)
    {
        Model = model;
        Id = model.Id;
        Username = model.Username;
        Status = new UserStatus();
        if (model.Online)
            Status.Type = UserStatusType.Online;
        else
            Status.Type = UserStatusType.Offline;
        if (model.Status != null)
        {
            Status.Text = model.Status.Text;
            if (model.Status != null && Enum.TryParse(model.Status.Presence, out UserStatusType ST))
                Status.Type = ST;
            else
                Status.Type = UserStatusType.Offline;
        }
        
        BotData = BotData.Create(model.Bot);
        Avatar = Attachment.Create(client, model.Avatar);
        Badges = new UserBadges(model.Badges);
        Flags = new UserFlags(model.Flags);
        Relationship = model.Relationship;
        
        
        Privileged = model.Privileged;
    }
    public bool HasBadge(UserBadgeTypes type)
        => Badges.Types.HasFlag(type);

    public string GetDefaultAvatarUrl()
        => Client.Config.ApiUrl + "users/" + Id + "/default_avatar";

    public string GetAvatarOrDefaultUrl()
        => Avatar != null ? Avatar.GetUrl() : GetDefaultAvatarUrl();

    internal void Update(PartialUserJson data)
    {
        if (data.avatar.HasValue)
            Avatar = Attachment.Create(Client, data.avatar.Value);
        
        if (data.online.HasValue)
        {
            if (data.online.Value)
                Status.Type = UserStatusType.Online;
            else
                Status.Type = UserStatusType.Offline;
        }

        if (data.status.HasValue)
        {
            if (data.status.Value != null)
            {
                Status.Text = data.status.Value.Text;
                if (data.status.Value != null && Enum.TryParse(data.status.Value.Presence, out UserStatusType ST))
                    Status.Type = ST;
                else
                    Status.Type = UserStatusType.Offline;
            }

        }
        
        if (this is SelfUser Self)
        {
            if (data.ProfileBackground.HasValue)
                Self.Background = Attachment.Create(Client, data.ProfileBackground.Value);

            if (data.ProfileContent.HasValue)
                Self.ProfileBio = data.ProfileContent.Value;
        }

        if (data.privileged.HasValue)
            Privileged = data.privileged.Value;

        if (data.Username.HasValue)
            Username = data.Username.Value;

        if (data.Badges.HasValue)
        {
            Badges.Raw = data.Badges.Value;
            Badges.Types = (UserBadgeTypes)Badges.Raw;
        }

        if (data.Flags.HasValue)
        {
            Flags.Raw = data.Flags.Value;
            Flags.Types = (UserFlagTypes)Flags.Raw;
        }
    }

    internal User Clone()
    {
        return (User)this.MemberwiseClone();
    }
}
public class UserStatus
{
    public string Text { get; internal set; }
    public UserStatusType Type { get; internal set; }
}
public class UserBadges
{
    internal UserBadges(ulong value)
    {
        Raw = value;
        Types = (UserBadgeTypes)Raw;
    }

    public ulong Raw { get; internal set; }

    public bool HasBadge(UserBadgeTypes type) => Types.HasFlag(type);

    internal UserBadgeTypes Types;
}
public class UserFlags
{
    internal UserFlags(ulong value)
    {
        Raw = value;
        Types = (UserFlagTypes)Raw;
    }
    public ulong Raw { get; internal set; }

    public bool HasFlag(UserFlagTypes type) => Types.HasFlag(type);

    internal UserFlagTypes Types;
}
public class BotData
{
    private BotData(UserBotJson json)
    {
        Owner = json.Owner;
    }

    internal static BotData? Create(UserBotJson json)
    {
        if (json != null)
            return new BotData(json);
        return null;
    }
    public string Owner { get; }
}
[Flags]
public enum UserBadgeTypes
{
    Developer = 1,
    Translator = 2,
    Supporter = 4,
    ResponsibleDisclosure = 8,
    Founder = 16,
    PlatformModeration = 32,
    ActiveSupporter = 64,
    Paw = 128,
    EarlyAdopter = 256,
    ReservedRelevantJokeBadge1 = 512,
    ReservedRelevantJokeBadge2 = 1024
}
[Flags]
public enum UserFlagTypes
{
    Suspended = 1,
    Deleted = 2,
    Banned = 4
}

public enum UserStatusType
{
    Offline, Online, Idle, Focus, Busy, Invisible
}
