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

        public ServerMember(RevoltClient client, ServerMemberJson sModel, UserJson uModel)
            : base(client)
        {
            Create(client, sModel, client.GetUser(uModel.Id) ?? new User(Client, uModel));
        }

        public ServerMember(RevoltClient client, ServerMemberJson sModel, User user)
             : base(client)
        {
            Create(client, sModel, user);
        }

        public void Create(RevoltClient client, ServerMemberJson sModel, User user)
        {
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
                }
            }
            else
            {
                RolesIds = new HashSet<string>();
            }
            User = user;
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
            }
        }
    }
}
