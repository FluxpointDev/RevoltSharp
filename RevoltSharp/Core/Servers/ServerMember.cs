using System.Collections.Generic;
using System.Linq;

namespace RevoltSharp
{
    public class ServerMember
    {
        public string Id
            => User.Id;

        public string ServerId;
        public string Nickname;
        public User User;
        public Attachment Avatar;
        public HashSet<string> RolesIds;

        internal static ServerMember Create(ServerMemberJson sjson, UserJson ujson)
        {
            return new ServerMember
            {
                ServerId = sjson.id.server,
                Avatar = sjson.avatar != null ? sjson.avatar.ToEntity() : null,
                Nickname = sjson.nickname,
                RolesIds = sjson.roles != null ? sjson.roles.ToHashSet() : new HashSet<string>(),
                User = ujson.ToEntity()
            };
        }
    }
}
