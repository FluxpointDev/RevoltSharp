using System;

namespace RevoltSharp;


/// <summary>
/// List of server permissions
/// </summary>
[Flags]
public enum ServerPermission : ulong
{
    /// <summary>
    /// Can create/delete channels and edit channel info for all channels in the server.
    /// </summary>
    ManageChannels = 1L << 0,
    /// <summary>
    /// Can edit the server info such as name and icon.
    /// </summary>
    ManageServer = 1L << 1,
    /// <summary>
    /// Can modify the permissions for all roles in the server below their current role rank.
    /// </summary>
    ManagePermissions = 1L << 2,
    /// <summary>
    /// Can create, edit and delete all roles in the server.
    /// </summary>
    ManageRoles = 1L << 3,
    /// <summary>
    /// Can create and delete emojis in the server.
    /// </summary>
    ManageCustomisation = 1L << 4,
    /// <summary>
    /// Can kick members from the server.
    /// </summary>
    KickMembers = 1L << 6,
    /// <summary>
    /// Can ban members from the server.
    /// </summary>
    BanMembers = 1L << 7,
    /// <summary>
    /// Can mute members in the server from chatting.
    /// </summary>
    TimeoutMembers = 1L << 8,
    /// <summary>
    /// Can give other members's a role that is below their current role rank.
    /// </summary>
    AssignRoles = 1L << 9,
    /// <summary>
    /// Can change their own nickname in the server.
    /// </summary>
    ChangeNickname = 1L << 10,
    /// <summary>
    /// Can change the nickname of other members in the server.
    /// </summary>
    ManageNicknames = 1L << 11,
    /// <summary>
    /// Can change their own avatar in the server.
    /// </summary>
    ChangeAvatar = 1L << 12,
    /// <summary>
    /// Can reset other member's avatars in the server.
    /// </summary>
    ManageAvatars = 1L << 13,
    /// <summary>
    /// Can view all channels in the server.
    /// </summary>
    ViewChannels = 1L << 20,
    /// <summary>
    /// Can send messages to all channels in the server.
    /// </summary>
    SendMessages = 1L << 22,
    /// <summary>
    /// Can moderate messages in all channels by deleting them or removing reactions.
    /// </summary>
    ManageMessages = 1L << 23,
    /// <summary>
    /// Can create, edit and delete external webhooks in all channels.
    /// </summary>
    ManageWebhooks = 1L << 24,
    /// <summary>
    /// Can create server invites to all channels in the server.
    /// </summary>
    CreateInvites = 1L << 25,
    /// <summary>
    /// Can send embed messages to all channels in the server
    /// </summary>
    SendEmbeds = 1L << 26,
    /// <summary>
    /// Can upload files and images to all channels in the server.
    /// </summary>
    UploadFiles = 1L << 27,
    /// <summary>
    /// Can send fake author messages in the channel for use with bridged chats from other platforms to all channels in the server.
    /// </summary>
    Masquerade = 1L << 28,
    /// <summary>
    /// Can add reactions to all channels with messages in the server.
    /// </summary>
    AddReactions = 1L << 29,
    /// <summary>
    /// Can connect to all voice channels in the server.
    /// </summary>
    VoiceConnect = 1L << 30,
    /// <summary>
    /// Can speak in all voice channels in the server.
    /// </summary>
    VoiceSpeak = 1L << 31,
    /// <summary>
    /// Can use video in all voice channels in the server.
    /// </summary>
    VoiceVideo = 1L << 32,
    /// <summary>
    /// Can mute other members in all voice channels in the server.
    /// </summary>
    VoiceMuteMembers = 1L << 33,
    /// <summary>
    /// Can deafen other members in all voice channels in the server.
    /// </summary>
    VoiceDeafenMembers = 1L << 34,
    /// <summary>
    /// Can move other members in all voice channels to other voice channels in the server.
    /// </summary>
    VoiceMoveMembers = 1L << 35
}