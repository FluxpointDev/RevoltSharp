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
        ManageChannels = 1 << 0,
        ManageServer = 1 << 1,
        ManagePermissions = 1 << 2,
        ManageRoles = 1 << 3,
        ManageCustomisation = 1 << 4,
        KickMembers = 1 << 6,
        BanMembers = 1 << 7,
        TimeoutMembers = 1 << 8,
        AssignRoles = 1 << 9,
        ChangeNickname = 1 << 10,
        ManageNicknames = 1 << 11,
        ChangeAvatar = 1 << 12,
        ManageAvatars = 1 << 13,

        ViewChannels = 1 << 20,
        ReadMessageHistory = 1 << 21,
        SendMessages = 1 << 22,
        ManageMessages = 1 << 23,
        ManageWebhooks = 1 << 24,
        CreateInvites = 1 << 25,
        SendEmbeds = 1 << 26,
        UploadFiles = 1 << 27,
        Masquerade = 1 << 28,
        AddReactions = 1 << 29,
        VoiceConnect = 1 << 30,
        VoiceSpeak = 1 << 31,
        VoiceVideo = 1 << 32,
        VoiceMuteMembers = 1 << 33,
        VoiceDeafenMembers = 1 << 34,
        VoiceMoveMembers = 1 << 35
    }
}
