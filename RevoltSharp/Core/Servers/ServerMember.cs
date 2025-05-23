﻿using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RevoltSharp;


/// <summary>
/// A user that is a member of a server.
/// </summary>
public class ServerMember : Entity
{
    /// <summary>
    /// User ID of the parent user object.
    /// </summary>
    public string Id => User.Id;

    /// <summary>
    /// Member ID of the user.
    /// </summary>
    public string MemberId { get; }

    /// <summary>
    /// UTC date time of when the user joined the server.
    /// </summary>

    public DateTimeOffset JoinedAt { get; internal set; }

    /// <summary>
    /// Server ID that the user is in.
    /// </summary>
    public string ServerId { get; internal set; }

    /// <summary>
    /// Server that the Member is in.
    /// </summary>
    /// <remarks>
    /// Will be <see langword="null" /> if using <see cref="ClientMode.Http"/>.
    /// </remarks>
    public Server? Server => Client.GetServer(ServerId);

    /// <summary>
    /// Custom server nickname that user has set.
    /// </summary>
    public string Nickname { get; internal set; }

    /// <summary>
    /// The parent user object of this member.
    /// </summary>
    public User User { get; internal set; }

    /// <summary>
    /// Get the current name of this user from the nickname, display name or username.
    /// </summary>
    public string CurrentName => !string.IsNullOrEmpty(Nickname) ? Nickname : User.CurrentName;

    /// <summary>
    /// The avatar attachment for the custom member's avatar.
    /// </summary>
    /// <remarks>
    /// Will be <see langword="null" /> if member has no avatar set.
    /// </remarks>
    public Attachment? ServerAvatar { get; internal set; }

    /// <inheritdoc cref="User.GetAvatarUrl"/>
    public string? GetAvatarUrl(AvatarSources which = AvatarSources.Any)
    {
        if (Avatar != null && (which | AvatarSources.Server) != 0)
            return Avatar.GetUrl();

        return User.GetAvatarUrl(which);
    }

    /// <summary>
    /// List of role IDs that the member has.
    /// </summary>
    public string[] RolesIds { get; internal set; }

    /// <summary>
    /// The member is timed out/muted with the specified date time.
    /// </summary>
    /// <remarks>
    /// Will be null if member is not timed out/muted.
    /// </remarks>
    public DateTime? Timeout { get; internal set; }

    /// <summary>
    /// The member is currently timed out/muted.
    /// </summary>
    /// <remarks>
    /// They will not be able to send messsages in the server.
    /// </remarks>
    public bool IsTimedOut => Timeout.HasValue;

    internal ConcurrentDictionary<string, Role> InternalRoles { get; set; } = new ConcurrentDictionary<string, Role>();

    /// <summary>
    /// Get a role the member has.
    /// </summary>
    /// <remarks>
    /// Will be <see langword="null" /> if using <see cref="ClientMode.Http"/> or no Role found.
    /// </remarks>
    public Role? GetRole(string roleId)
    {
        if (InternalRoles.TryGetValue(roleId, out Role role) && roleId.Contains(role.Id))
            return role;
        return null;
    }

    [JsonIgnore]
    public IReadOnlyCollection<Role> Roles
        => (IReadOnlyCollection<Role>)InternalRoles.Values;

    /// <summary>
    /// The member has these permissions in the server.
    /// </summary>
    public ServerPermissions Permissions { get; internal set; }

    public ChannelPermissions GetPermissions(ServerChannel channel)
    {
        if (channel.Server.OwnerId == Id)
            return new ChannelPermissions(channel.Server, ulong.MaxValue, 0);

        ulong resolvedPermissions = channel.Server.DefaultPermissions.Raw;

        resolvedPermissions = (resolvedPermissions & ~channel.DefaultPermissions.RawDenied) | channel.DefaultPermissions.RawAllowed;

        foreach (Role r in InternalRoles.Values.OrderByDescending(x => x.Rank))
        {
            if (channel.InternalRolePermissions.TryGetValue(r.Id, out ChannelPermissions cperm))
            {
                resolvedPermissions = (resolvedPermissions & ~cperm.RawDenied) | cperm.RawAllowed;
            }
        }
        return new ChannelPermissions(channel.Server, resolvedPermissions, 0);
    }

    #region UserProperties
    /// <inheritdoc cref="User.CreatedAt"/>
    [JsonIgnore]
    public DateTimeOffset CreatedAt => User.CreatedAt;

    /// <inheritdoc cref="User.Username"/>
    [JsonIgnore]
    public string Username => User.Username;

    /// <inheritdoc cref="User.Discriminator"/>
    [JsonIgnore]
    public string Discriminator => User.Discriminator;

    /// <inheritdoc cref="User.DisplayName"/>
    [JsonIgnore]
    public string? DisplayName => User.DisplayName;

    /// <inheritdoc cref="User.Tag"/>
    [JsonIgnore]
    public string Tag => User.Tag;

    /// <inheritdoc cref="User.Mention"/>
    [JsonIgnore]
    public string Mention => User.Mention;

    /// <inheritdoc cref="User.Status"/>
    [JsonIgnore]
    public UserStatus Status => User.Status;

    /// <inheritdoc cref="User.Avatar"/>
    [JsonIgnore]
    public Attachment? Avatar => User.Avatar;

    /// <inheritdoc cref="User.Badges"/>
    [JsonIgnore]
    public UserBadges Badges => User.Badges;

    /// <inheritdoc cref="User.Flags"/>
    [JsonIgnore]
    public UserFlags Flags => User.Flags;

    /// <inheritdoc cref="User.BotData"/>
    [JsonIgnore]
    public BotData? BotData => User.BotData;

    /// <inheritdoc cref="User.IsOnline"/>
    [JsonIgnore]
    public bool IsOnline => User.IsOnline;

    /// <inheritdoc cref="User.IsPrivileged"/>
    [JsonIgnore]
    public bool IsPrivileged => User.IsPrivileged;

    /// <inheritdoc cref="User.Relationship"/>
    [JsonIgnore]
    public UserRelationship Relationship => User.Relationship;

    /// <inheritdoc cref="User.IsBot"/>
    [JsonIgnore]
    public bool IsBot => User.IsBot;

    /// <inheritdoc cref="User.IsBlocked"/>
    [JsonIgnore]
    public bool IsBlocked => User.IsBlocked;

    /// <inheritdoc cref="User.MutualDMs"/>
    [JsonIgnore]
    public IReadOnlyCollection<DMChannel> MutualDMs => User.MutualDMs;

    /// <inheritdoc cref="User.MutualServers"/>
    [JsonIgnore]
    public IReadOnlyCollection<Server> MutualServers => User.MutualServers;

    /// <inheritdoc cref="User.MutualGroups"/>
    [JsonIgnore]
    public IReadOnlyCollection<GroupChannel> MutualGroups => User.MutualGroups;

    #endregion


    internal ServerMember(RevoltClient client, ServerMemberJson sModel, UserJson? uModel, User? user) : base(client)
    {
        User = user;
        if (user == null && uModel != null)
            User = new User(Client, uModel);

        JoinedAt = sModel.JoinedAt;
        ServerId = sModel.Id.Server;
        Nickname = sModel.Nickname;
        if (sModel.Timeout.HasValue)
            Timeout = sModel.Timeout.Value;
        ServerAvatar = Attachment.Create(client, sModel.Avatar);
        RolesIds = sModel.Roles != null ? sModel.Roles.Distinct().ToArray() : Array.Empty<string>();
        Server Server = client.GetServer(ServerId);
        if (Server != null)
        {
            InternalRoles = new ConcurrentDictionary<string, Role>(RolesIds.ToDictionary(x => x, x => Server.InternalRoles[x]));
            Permissions = new ServerPermissions(Server, this);
        }
        else
        {
            InternalRoles = new ConcurrentDictionary<string, Role>();
            Permissions = new ServerPermissions(Server, 0);
        }
    }

    [JsonIgnore]
    internal readonly SemaphoreSlim RoleLock = new SemaphoreSlim(1);

    internal void Update(PartialServerMemberJson json)
    {
        if (json.Nickname.HasValue)
            Nickname = json.Nickname.Value;

        if (json.Avatar.HasValue)
            ServerAvatar = Attachment.Create(Client, json.Avatar.Value);

        if (json.Roles.HasValue)
        {
            Server server = Client.GetServer(ServerId);
            RolesIds = json.Roles.Value.Distinct().ToArray();
            InternalRoles = new ConcurrentDictionary<string, Role>(RolesIds.ToDictionary(x => x, x => server.InternalRoles[x]));
            Permissions = new ServerPermissions(server, this);
        }

        if (json.Timeout.HasValue)
            Timeout = json.Timeout.Value;

        if (json.ClearTimeout)
            Timeout = null;
    }

    /// <summary> Returns a string that represents the current object.</summary>
    /// <returns> Nickname, Display name or Username </returns>
    public override string ToString()
    {
        return CurrentName;
    }
}