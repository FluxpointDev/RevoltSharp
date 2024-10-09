using System;

namespace RevoltSharp;


/// <summary>
/// List of channel permissions.
/// </summary>
[Flags]
public enum ChannelPermission : ulong
{
    /// <summary>
    /// Can edit the channel info, icon, description or delete the channel.
    /// </summary>
    ManageChannel = 1L << 0,
    /// <summary>
    /// Can change the permissions of the channel.
    /// </summary>
    ManagePermissions = 1L << 2,
    /// <summary>
    /// Can view the channel and messages.
    /// </summary>
    ViewChannel = 1L << 20,
    /// <summary>
    /// Can send messages in the channel.
    /// </summary>
    SendMessages = 1L << 22,
    /// <summary>
    /// Can moderate messages in the channel by deleting them or removing reactions.
    /// </summary>
    ManageMessages = 1L << 23,
    /// <summary>
    /// Can create, edit and delete external webhooks.
    /// </summary>
    ManageWebhooks = 1L << 24,
    /// <summary>
    /// Can create server invites for the channel.
    /// </summary>
    CreateInvites = 1L << 25,
    /// <summary>
    /// Can send embed messages in the channel.
    /// </summary>
    SendEmbeds = 1L << 26,
    /// <summary>
    /// Can upload files and images in the channel.
    /// </summary>
    UploadFiles = 1L << 27,
    /// <summary>
    /// Can send fake author messages in the channel for use with bridged chats from other platforms.
    /// </summary>
    Masquerade = 1L << 28,
    /// <summary>
    /// Can add reactions to messages in the channel.
    /// </summary>
    AddReactions = 1L << 29,
    /// <summary>
    /// Can connect to the voice channel.
    /// </summary>
    VoiceConnect = 1L << 30,
    /// <summary>
    /// Can speak in the voice channel.
    /// </summary>
    VoiceSpeak = 1L << 31,
    /// <summary>
    /// Can use video in the voice channel.
    /// </summary>
    VoiceVideo = 1L << 32,
    /// <summary>
    /// Can mute other members in the voice channel.
    /// </summary>
    VoiceMuteMembers = 1L << 33,
    /// <summary>
    /// Can deafen other members in the voice channel.
    /// </summary>
    VoiceDeafenMembers = 1L << 34,
    /// <summary>
    /// Can move other members in the voice channel to other voice channels.
    /// </summary>
    VoiceMoveMembers = 1L << 35
}