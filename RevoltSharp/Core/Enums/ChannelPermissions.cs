using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Linq;

namespace RevoltSharp;


/// <summary>
/// Permissions for the channel that the role can or can't use
/// </summary>
public class ChannelPermissions
{
    /// <summary>
    /// Get all channel permissions which can be given.
    /// </summary>
    public static ulong AllChannelPermissions = ulong.MaxValue;

    /// <summary>
    /// The server that these permissions are from.
    /// </summary>
    [JsonIgnore]
    public Server Server { get; internal set; }

    /// <summary>
    /// Raw permissions number that has been allowed for the role.
    /// </summary>
    public ulong RawAllowed { get; internal set; }

    /// <summary>
    /// Raw permissions number that has been denied for the role.
    /// </summary>
    public ulong RawDenied { get; internal set; }

    internal ChannelPermissions(Server server, PermissionsJson permissions)
    {
        Server = server;

        if (permissions == null)
            return;

        RawAllowed = permissions.Allowed;
        RawDenied = permissions.Denied;
    }

    internal ChannelPermissions(Server server, ulong allowed, ulong denied)
    {
        Server = server;
        RawAllowed = allowed;
        RawDenied = denied;
    }

    /// <summary>
    /// List of channel permissions as single enum values.
    /// </summary>
    /// <returns>List of <see cref="ChannelPermission"/></returns>
    public IEnumerable<ChannelPermission> ToList()
    {
        ChannelPermission perm = (ChannelPermission)RawAllowed;
        return Enum.GetValues(typeof(ChannelPermission))
        .Cast<ChannelPermission>().Where(m => perm.HasFlag(m));
    }

    /// <summary>
    /// Check if the channel role override has a specific channel permission. 
    /// </summary>
    /// <returns><see langword="bool" /></returns>
    public bool Has(ServerPermission permission)
    {
        ulong Flag = (ulong)permission;
        if ((RawDenied & Flag) == Flag)
            return false;

        return ((RawAllowed & Flag) == Flag);
    }

    /// <summary>
    /// Check if the channel role override has a specific channel permission. 
    /// </summary>
    /// <returns><see langword="bool" /></returns>
    public bool Has(ChannelPermission permission)
    {
        ulong Flag = (ulong)permission;
        if ((RawDenied & Flag) == Flag)
            return false;

        return ((RawAllowed & Flag) == Flag);
    }
    /// <summary>
    /// User can modify and delete this server channel.
    /// </summary>
    public bool ManageChannel => Has(ChannelPermission.ManageChannel);

    /// <summary>
    /// User can modify this channel's permissions.
    /// </summary>
    public bool ManagePermissions => Has(ChannelPermission.ManagePermissions);

    /// <summary>
    /// User can view this channel.
    /// </summary>
    public bool ViewChanel => Has(ChannelPermission.ViewChannel);

    /// <summary>
    /// User can send messages in this channel.
    /// </summary>
    public bool SendMessages => Has(ChannelPermission.SendMessages);

    /// <summary>
    /// User can delete messages in this channel from other users.
    /// </summary>
    public bool ManageMessages => Has(ChannelPermission.ManageMessages);

    /// <summary>
    /// User can create, delete and modify this channel's webhooks.
    /// </summary>
    public bool ManageWebhooks => Has(ChannelPermission.ManageWebhooks);

    /// <summary>
    /// User can create invites for this channel to invite other users to the server.
    /// </summary>
    public bool CreateInvites => Has(ChannelPermission.CreateInvites);

    /// <summary>
    /// User can embed links in this channel.
    /// </summary>
    public bool EmbedLinks => Has(ChannelPermission.SendEmbeds);

    /// <summary>
    /// User can upload files in this channel.
    /// </summary>
    public bool UploadFiles => Has(ChannelPermission.UploadFiles);

    /// <summary>
    /// User can send masquerade messages with a custom name and avatar in this channel.
    /// </summary>
    public bool Masquerade => Has(ChannelPermission.Masquerade);

    /// <summary>
    /// User can add reactions to messages in this channel.
    /// </summary>
    public bool AddReactions => Has(ChannelPermission.AddReactions);

    /// <summary>
    /// User can connect to this voice channel.
    /// </summary>
    public bool VoiceConnect => Has(ChannelPermission.VoiceConnect);

    /// <summary>
    /// User can speak in this voice channel.
    /// </summary>
    public bool VoiceSpeak => Has(ChannelPermission.VoiceSpeak);

    /// <summary>
    /// User can use video in this voice channel.
    /// </summary>
    public bool VoiceVideo => Has(ChannelPermission.VoiceVideo);

    /// <summary>
    /// User can mute other users in this voice channel.
    /// </summary>
    public bool VoiceMuteMembers => Has(ChannelPermission.VoiceMuteMembers);

    /// <summary>
    /// User can deafen other users in this voice channel.
    /// </summary>
    public bool VoiceDeafenMembers => Has(ChannelPermission.VoiceDeafenMembers);

    /// <summary>
    /// User can move other users to this voice channel.
    /// </summary>
    public bool VoiceMoveMembers => Has(ChannelPermission.VoiceMoveMembers);
}
