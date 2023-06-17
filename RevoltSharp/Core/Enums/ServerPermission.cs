using System;

namespace RevoltSharp;

/// <summary>
/// List of server permissions
/// </summary>
[Flags]
public enum ServerPermission : ulong
{
    ManageChannels = 1L << 0,
    ManageServer = 1L << 1,
    ManagePermissions = 1L << 2,
    ManageRoles = 1L << 3,
    ManageCustomisation = 1L << 4,
    KickMembers = 1L << 6,
    BanMembers = 1L << 7,
    TimeoutMembers = 1L << 8,
    AssignRoles = 1L << 9,
    ChangeNickname = 1L << 10,
    ManageNicknames = 1L << 11,
    ChangeAvatar = 1L << 12,
    ManageAvatars = 1L << 13,

    ViewChannels = 1L << 20,
    SendMessages = 1L << 22,
    ManageMessages = 1L << 23,
    ManageWebhooks = 1L << 24,
    CreateInvites = 1L << 25,
    SendEmbeds = 1L << 26,
    UploadFiles = 1L << 27,
    Masquerade = 1L << 28,
    AddReactions = 1L << 29,
    VoiceConnect = 1L << 30,
    VoiceSpeak = 1L << 31,
    VoiceVideo = 1L << 32,
    VoiceMuteMembers = 1L << 33,
    VoiceDeafenMembers = 1L << 34,
    VoiceMoveMembers = 1L << 35
}
