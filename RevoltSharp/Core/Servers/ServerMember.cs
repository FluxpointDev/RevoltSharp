using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RevoltSharp
{
    public class ServerMember : Entity
    {
        public string Id => User.Id;

        public string ServerId { get; internal set; }

        public Server? Server => Client.GetServer(ServerId);

        public string Nickname { get; internal set; }

        public User User { get; internal set; }

        public Attachment? ServerAvatar { get; internal set; }

        public string[] RolesIds { get; internal set; }

        public DateTime JoinedAt { get; internal set; }

        public DateTime? Timeout { get; internal set; }

        internal ConcurrentDictionary<string, Role> InternalRoles { get; set;  }  = new ConcurrentDictionary<string, Role>();

        public Role GetRole(string roleId)
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
        public string Username => User.Username;
        public string Status => User.Status;
        public Attachment? Avatar => User.Avatar;
        public UserBadges Badges => User.Badges;
        public BotData? BotData => User.BotData;
        public bool IsOnline => User.IsOnline;
        public bool Privileged => User.Privileged;
        public string Relationship => User.Relationship;
        public bool IsBot => User.IsBot;
        #endregion



        internal ServerMember(RevoltClient client, ServerMemberJson sModel, UserJson uModel, User user) : base(client)
        {
            User = user != null ? user : new User(Client, uModel);
            ServerId = sModel.Id.Server;
            Nickname = sModel.Nickname;
            JoinedAt = sModel.JoinedAt;
            if (sModel.Timeout.HasValue)
                Timeout = sModel.Timeout.Value;
            ServerAvatar = sModel.Avatar != null ? new Attachment(sModel.Avatar) : null;
            RolesIds = sModel.Roles != null ? sModel.Roles.ToArray() : new string[0];
            Server server = client.GetServer(ServerId);
            server.AddMember(this);
            InternalRoles = new ConcurrentDictionary<string, Role>(RolesIds.ToDictionary(x => x, x => server.InternalRoles[x]));
            Permissions = new ServerPermissions(server, this);
        }

        internal void Update(PartialServerMemberJson json)
        {
            if (json.Nickname.HasValue)
                Nickname = json.Nickname.Value;

            if (json.Avatar.HasValue)
                ServerAvatar = json.Avatar.Value == null ? null : new Attachment(json.Avatar.Value);

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
}
