using System;

namespace RevoltSharp;

/// <summary>
/// List of channel permissions
/// </summary>
[Flags]
public enum ChannelPermission : ulong
{
    ManageChannel = 1L << 0,
    ManagePermissions = 1L << 2,
    ViewChannel = 1L << 20,
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
