using System;

namespace RevoltSharp;

/// <summary>
/// List of channel permissions
/// </summary>
[Flags]
public enum ChannelPermission
{
    ManageChannel = 1 << 0,
    ManagePermissions = 1 << 2,
    ViewChannel = 1 << 20,
    //ReadMessageHistory = 1 << 21,
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
