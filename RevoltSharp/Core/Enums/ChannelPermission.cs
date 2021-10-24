using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp
{
    [Flags]
    public enum ChannelPermission
    {
        ViewChannel = 1 << 0,
        SendMessages = 1 << 1,
        ManageMessages = 1 << 2,
        ManageChannel = 1 << 3,
        VoiceCall = 1 << 4,
        CreateInvite = 1 << 5,
        EmbedLinks = 1 << 6,
        UploadFiles = 1 << 7
    }
}
