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
        DisplayName = model.DisplayName;
        Discriminator = model.Discriminator;
        Status = new UserStatus(model);
        BotData = BotData.Create(model.Bot);
        Avatar = Attachment.Create(client, model.Avatar);
        Badges = new UserBadges(model.Badges);
        Flags = new UserFlags(model.Flags);

        if (!string.IsNullOrEmpty(model.Relationship) && Enum.TryParse(model.Relationship, ignoreCase: true, out UserRelationship UR))
            Relationship = UR;
        else
            Relationship = UserRelationship.None;

        IsPrivileged = model.Privileged;
    }

    internal User(RevoltClient client, MessageWebhookJson model) : base(client, model.Id)
    {
        Username = model.Name!;
        Discriminator = "0000";
        Status = new UserStatus(null) { Type = UserStatusType.Online };
        Avatar = Attachment.Create(client, model.Avatar);
        Badges = new UserBadges(0);
        Flags = new UserFlags(0);
        Relationship = UserRelationship.None;
        IsWebhook = true;
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
    /// Unique username of the user.
    /// </summary>
    public string Username { get; internal set; }

    /// <summary>
    /// Unique identity number of the user.
    /// </summary>
    public string Discriminator { get; internal set; }

    /// <summary>
    /// Get the display name of the user.
    /// </summary>
    public string? DisplayName { get; internal set; }

    /// <summary>
    /// Get the display name or username of the user.
    /// </summary>
    public string CurrentName => DisplayName ?? Username;

    /// <summary>
    /// Get the username and discriminator of the user.
    /// </summary>
    public string Tag
        => $"{Username}#{Discriminator}";

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
    /// Gets the user's avatar.
    /// </summary>
    /// <param name="which">Which avatar to return.</param>
    /// <param name="size"></param>
    /// <returns>URL of the image</returns>
    public string? GetAvatarUrl(AvatarSources which = AvatarSources.Any, int? size = null)
    {
        if (Avatar != null && (which | AvatarSources.User) != 0)
            return Avatar.GetUrl(size);

        if ((which | AvatarSources.Default) != 0)
        {
            Conditions.ImageSizeLength(size, nameof(GetAvatarUrl));
            return $"{Client.Config.ApiUrl}users/{Id}/default_avatar{(size != null ? $"?size={size}" : null)}";
        }

        return null;
    }

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
    public bool IsPrivileged { get; internal set; }

    /// <summary>
    /// The relationship type of the user compared to the current user/bot account.
    /// </summary>
    public UserRelationship Relationship { get; internal set; }

    /// <summary>
    /// Is the user a bot account or webhook.
    /// </summary>
    public bool IsBot => IsWebhook ? true : BotData != null;

    /// <summary>
    /// Is the user a webhook.
    /// </summary>
    public bool IsWebhook { get; internal set; }

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
        if (data.Avatar.HasValue)
            Avatar = Attachment.Create(Client, data.Avatar.Value);

        if (this is SelfUser self && data.Profile.HasValue)
        {
            if (data.Profile.Value.Background.HasValue)
                self.Background = Attachment.Create(Client, data.Profile.Value.Background.Value);

            if (data.Profile.Value.Content.HasValue)
                self.ProfileBio = data.Profile.Value.Content.Value;
        }

        if (data.Privileged.HasValue)
            IsPrivileged = data.Privileged.Value;

        if (data.Username.HasValue)
            Username = data.Username.Value;

        if (data.Discriminator.HasValue)
            Discriminator = data.Discriminator.Value;

        if (data.DisplayName.HasValue)
            DisplayName = data.DisplayName.Value;

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

    /// <summary> Returns a string that represents the current object.</summary>
    /// <returns> Name#0001 </returns>
    public override string ToString()
    {
        return Tag;
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