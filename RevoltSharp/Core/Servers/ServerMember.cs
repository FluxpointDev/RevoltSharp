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

        public HashSet<string> RolesIds { get; internal set; }

        public ConcurrentDictionary<string, Role> Roles { get; internal set;  }  = new ConcurrentDictionary<string, Role>();

        public ServerPermissions Permissions { get; internal set; }

        internal ServerMember(RevoltClient client, ServerMemberJson sModel, UserJson uModel, User user) : base(client)
        {
            User = user != null ? user : new User(Client, uModel);
            if (sModel != null)
            {
                ServerId = sModel.Id.Server;
                Nickname = sModel.Nickname;
                ServerAvatar = sModel.Avatar != null ? new Attachment(client, sModel.Avatar) : null;
                RolesIds = sModel.Roles != null ? sModel.Roles.ToHashSet() : new HashSet<string>();
                if (client.WebSocket != null)
                {
                    Server server = client.GetServer(ServerId);
                    Roles = new ConcurrentDictionary<string, Role>(RolesIds.ToDictionary(x => x, x => server.Roles[x]));
                    Permissions = new ServerPermissions(server, this);
                }
            }
            else
            {
                RolesIds = new HashSet<string>();
            }
            if (client.WebSocket != null)
            {
                Server server = client.GetServer(ServerId);
                server.AddMember(this);
            }
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
                Roles = new ConcurrentDictionary<string, Role>(RolesIds.ToDictionary(x => x, x => server.Roles[x]));
                Permissions = new ServerPermissions(server, this);
            }
        }
    }
}
