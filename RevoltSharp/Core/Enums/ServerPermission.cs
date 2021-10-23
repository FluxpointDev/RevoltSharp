using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp
{
    [Flags]
    public enum ServerPermission
    {
        ViewChannels = 1 << 0,
        ManageRoles = 1 << 1,
        ManageChannels = 1 << 2,
        ManageServer = 1 << 3,
        KickMembers = 1 << 4,
        BanMembers = 1 << 5,
        ChangeNickname = 1 << 12,
        ManageNicknames = 1 << 13,
        ChangeAvatar = 1 << 14,
        RemoveAvatars = 1 << 15,
    }
}
