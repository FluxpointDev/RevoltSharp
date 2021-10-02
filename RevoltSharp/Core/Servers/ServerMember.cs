using System.Collections.Generic;
using System.Linq;

namespace RevoltSharp
{
    public class ServerMember : Entity
    {
        public string Id => User.Id;

        public string ServerId { get; }

        public string Nickname { get; }

        public User User { get; }

        public Attachment ServerAvatar { get; }

        public HashSet<string> RolesIds { get; }

        public ServerMember(RevoltClient client, ServerMemberJson sModel, UserJson uModel)
            : base(client)
        {
            ServerId = sModel.Id.Server;
            Nickname = sModel.Nickname;
            ServerAvatar = sModel.Avatar != null ? new Attachment(client, sModel.Avatar) : null;
            RolesIds = sModel.Roles != null ? sModel.Roles.ToHashSet() : new HashSet<string>();
            User = new User(client, uModel);
        }
    }
}
