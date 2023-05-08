using Newtonsoft.Json;
using RevoltSharp.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RevoltSharp;

public class ServerMember : Entity
{
    public string Id => User.Id;

    public string MemberId { get; }

    public DateTimeOffset JoinedAt { get; internal set; }

    public string ServerId { get; internal set; }

    /// <summary>
    /// Server that the Member is from.
    /// </summary>
    /// <remarks>
    /// Will be <see langword="null" /> if using <see cref="ClientMode.Http"/>.
    /// </remarks>
    public Server? Server => Client.GetServer(ServerId);

    public string Nickname { get; internal set; }

    public User User { get; internal set; }

    public string CurrentName => !string.IsNullOrEmpty(Nickname) ? Nickname : User.Username;

    public Attachment? ServerAvatar { get; internal set; }

    public string GetDefaultAvatarUrl()
        => Client.Config.ApiUrl + "users/" + Id + "/default_avatar";

    public string GetServerAvatarUrl()
        => Avatar != null ? Avatar.GetUrl() : string.Empty;

    public string GetServerAvatarOrDefaultUrl()
        => Avatar != null ? Avatar.GetUrl() : GetDefaultAvatarUrl();

    public string[] RolesIds { get; internal set; }

    public DateTime? Timeout { get; internal set; }

    public bool IsTimedOut => Timeout.HasValue;

    internal ConcurrentDictionary<string, Role> InternalRoles { get; set;  }  = new ConcurrentDictionary<string, Role>();

    /// <summary>
    /// Get a role from the user.
    /// </summary>
    /// <remarks>
    /// Will be <see langword="null" /> if using <see cref="ClientMode.Http"/> or no Role found.
    /// </remarks>
    public Role? GetRole(string roleId)
    {
        if (InternalRoles.TryGetValue(roleId, out Role role))
            return role;
        return null;
    }

    [JsonIgnore]
    public IReadOnlyCollection<Role> Roles
        => (IReadOnlyCollection<Role>)InternalRoles.Values;

    public ServerPermissions Permissions { get; internal set; }

    #region UserProperties
    [JsonIgnore]
    public string Username => User.Username;

    [JsonIgnore]
    public UserStatus Status => User.Status;

    [JsonIgnore]
    public Attachment? Avatar => User.Avatar;

    [JsonIgnore]
    public UserBadges Badges => User.Badges;

    [JsonIgnore]
    public UserFlags Flags => User.Flags;

    [JsonIgnore]
    public BotData? BotData => User.BotData;

    [JsonIgnore]
    public bool IsOnline => User.IsOnline;

    [JsonIgnore]
    public bool Privileged => User.Privileged;

    [JsonIgnore]
    public UserRelationship Relationship => User.Relationship;

    [JsonIgnore]
    public bool IsBot => User.IsBot;
    #endregion



    internal ServerMember(RevoltClient client, ServerMemberJson sModel, UserJson uModel, User user) : base(client)
    {
        MemberId = sModel.Id.User;
        if (Ulid.TryParse(MemberId, out Ulid UID))
            JoinedAt = UID.Time;
        User = user != null ? user : new User(Client, uModel);
        ServerId = sModel.Id.Server;
        Nickname = sModel.Nickname;
        //JoinedAt = sModel.JoinedAt;
        if (sModel.Timeout.HasValue)
            Timeout = sModel.Timeout.Value;
        ServerAvatar = Attachment.Create(client, sModel.Avatar);
        RolesIds = sModel.Roles != null ? sModel.Roles.ToArray() : new string[0];
        Server Server = client.GetServer(ServerId);
        if (Server != null)
        {
            InternalRoles = new ConcurrentDictionary<string, Role>(RolesIds.ToDictionary(x => x, x => Server.InternalRoles[x]));
            Permissions = new ServerPermissions(Server, this);
        }
        else
        {
            InternalRoles = new ConcurrentDictionary<string, Role>();
            Permissions = new ServerPermissions(0);
        }
    }

    internal void Update(PartialServerMemberJson json)
    {
        if (json.Nickname.HasValue)
            Nickname = json.Nickname.Value;

        if (json.Avatar.HasValue)
            ServerAvatar = Attachment.Create(Client, json.Avatar.Value);

        if (json.Roles.HasValue)
        {
            RolesIds = json.Roles.Value;
            Server server = Client.GetServer(ServerId);
            InternalRoles = new ConcurrentDictionary<string, Role>(RolesIds.ToDictionary(x => x, x => server.InternalRoles[x]));
            Permissions = new ServerPermissions(server, this);
        }

        if (json.Timeout.HasValue)
            Timeout = json.Timeout.Value;

        if (json.ClearTimeout)
            Timeout = null;
    }
}
