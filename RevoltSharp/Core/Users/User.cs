
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RevoltSharp;

/// <summary>
/// A Revolt user with various data.
/// </summary>
public class User : CreatedEntity
{
    /// <summary>
    /// The User ID for this user.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Username of the user.
    /// </summary>
    public string Username { get; internal set; }

    /// <summary>
    /// Status mode and text for the user.
    /// </summary>
    public UserStatus Status { get; internal set; }

    /// <summary>
    /// Avatar attachment for this user.
    /// </summary>
    /// <remarks>
    /// Will be <see langword="null" /> if the user has no custom avatar set.
    /// </remarks>
    public Attachment? Avatar { get; internal set; }

    /// <summary>
    /// Get the default revolt avatar url for this user.
    /// </summary>
    /// <returns>URL of the image (No extension)</returns>
    public string GetDefaultAvatarUrl()
        => Client.Config.ApiUrl + "users/" + Id + "/default_avatar";

    /// <summary>
    /// Get the user's custom avatar url, may be empty.
    /// </summary>
    /// <returns>URL of the image</returns>
    public string GetAvatarUrl()
        => Avatar != null ? Avatar.GetUrl() : string.Empty;

    /// <summary>
    /// Get the user's custom avatar url or the default revolt avatar url. 
    /// </summary>
    /// <returns>URL of the image</returns>
    public string GetAvatarOrDefaultUrl()
        => Avatar != null ? Avatar.GetUrl() : GetDefaultAvatarUrl();

    /// <summary>
    /// Cool badges that the user has.
    /// </summary>
    public UserBadges Badges { get; }

    /// <summary>
    /// User has been marked with system settings such as <see cref="UserFlagType.Suspended"/> or <see cref="UserFlagType.Deleted"/>.
    /// </summary>
    public UserFlags Flags { get; internal set; }

    /// <summary>
    /// The data for a bot account with owner property.
    /// </summary>
    /// <remarks>
    /// Will be <see langword="null" /> if the <see cref="User"/> is not a bot account
    /// </remarks>
    public BotData? BotData { get; }

    /// <summary>
    /// Is the user currently online.
    /// </summary>
    /// <remarks>
    /// Will be <see langword="false" /> if status is also <see cref="UserStatusType.Invisible"/>
    /// </remarks>
    public bool IsOnline => (Status.Type != UserStatusType.Offline && Status.Type != UserStatusType.Invisible);

    /// <summary>
    /// Is the user a Revolt instance administrator.
    /// </summary>
    public bool Privileged { get; internal set; }

    /// <summary>
    /// The relationship type of the user compared to the current user/bot account.
    /// </summary>
    public UserRelationship Relationship { get; internal set; }

    /// <summary>
    /// Is the user a bot account.
    /// </summary>
    public bool IsBot => BotData != null;

    /// <summary>
    /// Is ther user blocked by the current user/bot account.
    /// </summary>
    public bool IsBlocked => (Relationship == UserRelationship.Blocked || Relationship == UserRelationship.BlockedOther);

    internal ConcurrentDictionary<string, Server> InternalMutualServers { get; set; } = new ConcurrentDictionary<string, Server>();

    /// <summary>
    /// Known mutual servers that this user has for the current user/bot account.
    /// </summary>
    /// <remarks>
    /// This may not be fully accurate.
    /// </remarks>
    [JsonIgnore]
    public IReadOnlyCollection<Server> MutualServers
        => (IReadOnlyCollection<Server>)InternalMutualServers.Values;

    internal ConcurrentDictionary<string, GroupChannel> InternalMutualGroups { get; set; } = new ConcurrentDictionary<string, GroupChannel>();

    /// <summary>
    /// Known mutual group channels that this user has for the current user/bot account.
    /// </summary>
    /// <remarks>
    /// This may not be fully accurate.
    /// </remarks>
    [JsonIgnore]
    public IReadOnlyCollection<GroupChannel> MutualGroups
       => (IReadOnlyCollection<GroupChannel>)InternalMutualGroups.Values;

    /// <summary>
    /// Does the user have mutual servers, groups or DMs for the current user/bot account.
    /// </summary>
    /// <remarks>
    /// This may not be fully accurate.
    /// </remarks>
    internal bool HasMutuals
       => InternalMutualGroups.Any() || InternalMutualServers.Any();

    internal User(RevoltClient client, UserJson model)
        : base(client, model.Id)
    {
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
            if (model.Status != null && Enum.TryParse(model.Status.Presence, ignoreCase: true, out UserStatusType ST))
                Status.Type = ST;
            else
                Status.Type = UserStatusType.Offline;
        }
        else
            Status.Type = UserStatusType.Offline;

        BotData = BotData.Create(model.Bot);
        Avatar = Attachment.Create(client, model.Avatar);
        Badges = new UserBadges(model.Badges);
        Flags = new UserFlags(model.Flags);

        if (!string.IsNullOrEmpty(model.Relationship) && Enum.TryParse(model.Relationship, ignoreCase: true, out UserRelationship UR))
            Relationship = UR;
        else
            Relationship = UserRelationship.None;

        Privileged = model.Privileged;
    }

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

        if (data.status.HasValue && data.status.Value != null)
        {
            Status.Text = data.status.Value.Text;
            if (data.status.Value != null && Enum.TryParse(data.status.Value.Presence, ignoreCase: true, out UserStatusType ST))
                Status.Type = ST;
            else
                Status.Type = UserStatusType.Offline;
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
            Badges.Types = (UserBadgeType)Badges.Raw;
        }

        if (data.Flags.HasValue)
        {
            Flags.Raw = data.Flags.Value;
            Flags.Types = (UserFlagType)Flags.Raw;
        }
    }

    internal User Clone()
    {
        return (User)this.MemberwiseClone();
    }
}

/// <summary>
/// User status mode and presence text.
/// </summary>
public class UserStatus
{
    /// <summary>
    /// Custom text status for the user.
    /// </summary>
    public string Text { get; internal set; }

    /// <summary>
    /// Status mode for the user.
    /// </summary>
    public UserStatusType Type { get; internal set; }
}

/// <summary>
/// Cool badges the user has.
/// </summary>
public class UserBadges
{
    internal UserBadges(ulong value)
    {
        Raw = value;
        Types = (UserBadgeType)Raw;
    }

    /// <summary>
    /// Not recommended to use, use <see cref="Has(UserBadgeType)"/> instead.
    /// </summary>
    public ulong Raw { get; internal set; }

    /// <summary>
    /// Check if a user has a badge.
    /// </summary>
    /// <param name="type">The type of badge to check</param>
    /// <returns><see langword="true" /> if user has this badge otherwise <see langword="false" /></returns>
    public bool Has(UserBadgeType type) => Types.HasFlag(type);

    internal UserBadgeType Types;
}

/// <summary>
/// System flags set for the user.
/// </summary>
public class UserFlags
{
    internal UserFlags(ulong value)
    {
        Raw = value;
        Types = (UserFlagType)Raw;
    }

    /// <summary>
    /// Not recommended to use, use <see cref="Has(UserFlagType)"/> instead.
    /// </summary>
    public ulong Raw { get; internal set; }

    /// <summary>
    /// Check if the user has a flag.
    /// </summary>
    /// <param name="type">The type of system flag to check</param>
    /// <returns><see langword="true" /> if user has the flag otherwise <see langword="false" /></returns>
    public bool Has(UserFlagType type) => Types.HasFlag(type);

    internal UserFlagType Types;
}

/// <summary>
/// Data for the bot account that this user is.
/// </summary>
public class BotData
{
    private BotData(UserBotJson json)
    {
        OwnerId = json.Owner;
    }

    internal static BotData? Create(UserBotJson json)
    {
        if (json != null)
            return new BotData(json);
        return null;
    }

    /// <summary>
    /// Owner ID of the bot account.
    /// </summary>
    public string OwnerId { get; }


}

/// <summary>
/// Cool badges for users :)
/// </summary>
[Flags]
public enum UserBadgeType
{
    /// <summary>
    /// User is a Revolt developer that works on Revolt magic
    /// </summary>
    Developer = 1,

    /// <summary>
    /// User has helped translate Revolt or other Revolt related stuff.
    /// </summary>
    Translator = 2,

    /// <summary>
    /// User has supported the project by donating.
    /// </summary>
    Supporter = 4,

    /// <summary>
    /// User has disclosed a major bug or security issue.
    /// </summary>
    ResponsibleDisclosure = 8,

    /// <summary>
    /// Hi insert :)
    /// </summary>
    Founder = 16,

    /// <summary>
    /// User has the power to moderate the Revolt instance.
    /// </summary>
    PlatformModeration = 32,

    /// <summary>
    /// Active support for the Revolt project.
    /// </summary>
    ActiveSupporter = 64,

    /// <summary>
    /// OwO
    /// </summary>
    Paw = 128,

    /// <summary>
    /// User was an early member/tester of the Revolt project.
    /// </summary>
    EarlyAdopter = 256,

    /// <summary>
    /// Haha funny
    /// </summary>
    ReservedRelevantJokeBadge1 = 512,

    /// <summary>
    /// Haha memes
    /// </summary>
    ReservedRelevantJokeBadge2 = 1024
}

/// <summary>
/// System flags for the Revolt instance.
/// </summary>
[Flags]
public enum UserFlagType
{
    /// <summary>
    /// User has been suspended from using Revolt.
    /// </summary>
    Suspended = 1,

    /// <summary>
    /// User has been deleted from the Revolt instance.
    /// </summary>
    Deleted = 2,

    /// <summary>
    /// User has been banned from the Revolt instance.
    /// </summary>
    Banned = 4
}

/// <summary>
/// Status mode for the user.
/// </summary>
public enum UserStatusType
{
    /// <summary>
    /// User is not online on Revolt.
    /// </summary>
    Offline, 
    
    /// <summary>
    /// User is online and using Revolt.
    /// </summary>
    Online,
    
    /// <summary>
    /// User is away from their computer.
    /// </summary>
    Idle,
    
    /// <summary>
    /// User is focused on a task but is available.
    /// </summary>
    Focus,
    
    /// <summary>
    /// Do not FK WITH THIS USER.
    /// </summary>
    Busy,
    
    /// <summary>
    /// Who you gonna call? Ghost busters!
    /// </summary>
    Invisible
}

/// <summary>
/// Relationship type compared to the current user/bot account.
/// </summary>
public enum UserRelationship
{
    /// <summary>
    /// Default type
    /// </summary>
    None, 
    
    /// <summary>
    /// Idk shrug
    /// </summary>
    User,
    
    /// <summary>
    /// User is a friend
    /// </summary>
    Friend, 
    
    /// <summary>
    /// User needs to accept a friend request
    /// </summary>
    Outgoing,
    
    /// <summary>
    /// You need to accept a friend request from the user
    /// </summary>
    Incoming,
    
    /// <summary>
    /// The current user/bot has blocked this user
    /// </summary>
    Blocked,
    
    /// <summary>
    /// The user has blocked the current user/bot
    /// </summary>
    BlockedOther
}
