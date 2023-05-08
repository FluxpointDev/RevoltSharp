
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RevoltSharp;

public class User : Entity
{
    public string Id { get; }

    public string Username { get; }

    public string Status { get; internal set; }

    public Attachment? Avatar { get; internal set; }

    public UserBadges Badges { get; }

    public UserFlags Flags { get; internal set; }

    public BotData? BotData { get; }

    public bool IsOnline { get; }

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
        : base(client)
    {
        Model = model;
        Id = model.Id;
        Username = model.Username;
        BotData = model.Bot != null ? new BotData { Owner = model.Bot.Owner } : null;
        Avatar = model.Avatar != null ? new Attachment(model.Avatar) : null;
        Badges = new UserBadges(model.Badges);
        Flags = new UserFlags(model.Flags);
        IsOnline = model.Online;
        Relationship = model.Relationship;
        Status = model.Status?.Text;
        Privileged = model.Privileged;
    }
    public bool HasBadge(UserBadgeTypes type)
        => Badges.Types.HasFlag(type);

    internal void Update(PartialUserJson data)
    {
        if (data.avatar.HasValue)
            Avatar = data.avatar.Value != null ? new Attachment(data.avatar.Value) : null;
        if (data.status.HasValue)
            Status = data.status.Value != null ? data.status.Value.Text : null;
        if (this is SelfUser Self)
        {
            if (data.ProfileBackground.HasValue)
                Self.Background = data.ProfileBackground.Value != null ? new Attachment(data.ProfileBackground.Value) : null;

            if (data.ProfileContent.HasValue)
                Self.ProfileBio = data.ProfileContent.Value;
        }

        if (data.privileged.HasValue)
            Privileged = data.privileged.Value;
    }

    internal User Clone()
    {
        return (User)this.MemberwiseClone();
    }
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
    internal BotData() { }
    public string Owner { get; internal set; }
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
    Online, Idle, Busy, Invisible
}
