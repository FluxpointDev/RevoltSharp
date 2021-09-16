using System.Collections.Generic;
using System.Linq;

namespace RevoltSharp
{
    public class ServerMember
    {
        public string Id
            => User.Id;

        public string ServerId { get; internal set; }
        public string Nickname { get; internal set; }
        public User User { get; internal set; }
        public Attachment Avatar { get; internal set; }
        public HashSet<string> RolesIds { get; internal set; }

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
