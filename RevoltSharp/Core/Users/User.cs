
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
    internal User(RevoltClient client, UserJson model) : base(client, model.Id)
    {
        Username = model.Username;
        Status = new UserStatus(model);
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

    /// <summary>
    /// Id of the user.
    /// </summary>
    public new string Id => base.Id;

    /// <summary>
    /// Date of when the user was created.
    /// </summary>
    public new DateTimeOffset CreatedAt => base.CreatedAt;

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

    internal ConcurrentDictionary<string, DMChannel> InternalMutualDMs { get; set; } = new ConcurrentDictionary<string, DMChannel>();

    /// <summary>
    /// Known mutual DM channels for the current user/bot account.
    /// </summary>
    [JsonIgnore]
    public IReadOnlyCollection<DMChannel> MutualDMs
        => (IReadOnlyCollection<DMChannel>)InternalMutualDMs.Values;

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
       => InternalMutualGroups.Any() || InternalMutualServers.Any() || InternalMutualDMs.Any();

    internal void Update(PartialUserJson data)
    {
        if (data.avatar.HasValue)
            Avatar = Attachment.Create(Client, data.avatar.Value);

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
