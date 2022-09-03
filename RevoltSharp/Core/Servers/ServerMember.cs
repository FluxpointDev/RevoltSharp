using Optional.Unsafe;
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

        public string Nickname { get; internal set; }

        public User User { get; internal set; }

        public Attachment ServerAvatar { get; internal set; }

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

        public IReadOnlyCollection<Role> Roles
            => (IReadOnlyCollection<Role>)InternalRoles.Values;


        public ServerPermissions Permissions { get; internal set; }

        internal ServerMember(RevoltClient client, ServerMemberJson sModel, UserJson uModel, User user) : base(client)
        {
            User = user != null ? user : new User(Client, uModel);
            ServerId = sModel.Id.Server;
            Nickname = sModel.Nickname;
            JoinedAt = sModel.JoinedAt;
            if (sModel.Timeout.HasValue)
                Timeout = sModel.Timeout.ValueOrDefault();
            ServerAvatar = sModel.Avatar != null ? new Attachment(client, sModel.Avatar) : null;
            RolesIds = sModel.Roles != null ? sModel.Roles.ToArray() : new string[0];
            Server server = client.GetServer(ServerId);
            server.AddMember(this);
            InternalRoles = new ConcurrentDictionary<string, Role>(RolesIds.ToDictionary(x => x, x => server.InternalRoles[x]));
            Permissions = new ServerPermissions(server, this);
        }

        internal void Update(PartialServerMemberJson json)
        {
            if (json.Nickname.HasValue)
                Nickname = json.Nickname.ValueOrDefault();

            if (json.Avatar.HasValue)
                ServerAvatar = json.Avatar.ValueOrDefault() == null ? null : new Attachment(Client, json.Avatar.ValueOrDefault());

            if (json.Roles.HasValue)
            {
                RolesIds = json.Roles.ValueOrDefault();
                Server server = Client.GetServer(ServerId);
                InternalRoles = new ConcurrentDictionary<string, Role>(RolesIds.ToDictionary(x => x, x => server.InternalRoles[x]));
                Permissions = new ServerPermissions(server, this);
            }

            if (json.Timeout.HasValue)
                Timeout = json.Timeout.ValueOrDefault();

            if (json.ClearTimeout)
                Timeout = null;
        }
    }
}
