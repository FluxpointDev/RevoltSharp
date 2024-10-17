using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Linq;

namespace RevoltSharp;


/// <summary>
/// Permissions for the server that members can or can't use.
/// </summary>
public class ServerPermissions
{
    /// <summary>
    /// Get all server permissions which can be given.
    /// </summary>
    public static ulong AllServerPermissions = ulong.MaxValue;

    /// <summary>
    /// The server that these permissions are from.
    /// </summary>
    [JsonIgnore]
    public Server Server { get; internal set; }

    internal ServerPermissions(Server server, ulong permissions)
    {
        Server = server;
        Raw = permissions;
    }

    internal ServerPermissions(Server server, ServerMember member)
    {
        Server = server;

        if (server != null && server.OwnerId == member.Id)
        {
            Raw = ulong.MaxValue;
        }
        else
        {
            ulong resolvedServer = server.DefaultPermissions.Raw;
            foreach (Role r in member.InternalRoles.Values)
            {
                resolvedServer |= r.Permissions.Raw;
            }
            Raw = resolvedServer;
        }
    }

    /// <summary>
    /// List of server permissions as single enum values.
    /// </summary>
    /// <returns>List of <see cref="ServerPermission"/></returns>
    public IEnumerable<ServerPermission> ToList()
    {
        ServerPermission perm = (ServerPermission)Raw;
        return Enum.GetValues(typeof(ServerPermission))
        .Cast<ServerPermission>().Where(m => perm.HasFlag(m));
    }


    /// <summary>
    /// Raw permissions number for the server.
    /// </summary>
    public ulong Raw { get; internal set; }

    /// <summary>
    /// User can modify and delete server channels.
    /// </summary>
    public bool ManageChannels => Has(ChannelPermission.ManageChannel);

    /// <summary>
    /// User can modify server settings and invite bots.
    /// </summary>
    public bool ManageServer => Has(ServerPermission.ManageServer);

    /// <summary>
    /// User can modify role permissions and channel permissions.
    /// </summary>
    public bool ManagePermissions => Has(ServerPermission.ManagePermissions);
    
    /// <summary>
    /// User can create, delete and modify roles.
    /// </summary>
    public bool ManageRoles => Has(ServerPermission.ManageRoles);

    /// <summary>
    /// User can create, delete and modify emojis.
    /// </summary>
    public bool ManageCustomisation => Has(ServerPermission.ManageCustomisation);
    
    /// <summary>
    /// User can kick users from the server
    /// </summary>
    public bool KickMembers => Has(ServerPermission.KickMembers);

    /// <summary>
    /// User can ban users from the server.
    /// </summary>
    public bool BanMembers => Has(ServerPermission.BanMembers);

    /// <summary>
    /// User can mute/timeout a user from chatting in the server.
    /// </summary>
    public bool TimeoutMembers => Has(ServerPermission.TimeoutMembers);

    /// <summary>
    /// User can give other users a role.
    /// </summary>
    public bool AssignRoles => Has(ServerPermission.AssignRoles);

    /// <summary>
    /// User can change their server nickname.
    /// </summary>
    public bool ChangeNickname => Has(ServerPermission.ChangeNickname);

    /// <summary>
    /// User can modify other user's server nickname.
    /// </summary>
    public bool ManageNicknames => Has(ServerPermission.ManageNicknames);

    /// <summary>
    /// User can change their server avatar.
    /// </summary>
    public bool ChangeAvatar => Has(ServerPermission.ChangeAvatar);

    /// <summary>
    /// User can reset another user's avatar.
    /// </summary>
    public bool ManageAvatars => Has(ServerPermission.ManageAvatars);

    /// <summary>
    /// User can view channels in the server.
    /// </summary>
    public bool ViewChannels => Has(ChannelPermission.ViewChannel);

    /// <summary>
    /// User can send messages in channels.
    /// </summary>
    public bool SendMessages => Has(ChannelPermission.SendMessages);

    /// <summary>
    /// User can delete messages in channels from other users.
    /// </summary>
    public bool ManageMessages => Has(ChannelPermission.ManageMessages);

    /// <summary>
    /// User can create, delete and modify channel webhooks.
    /// </summary>
    public bool ManageWebhooks => Has(ChannelPermission.ManageWebhooks);

    /// <summary>
    /// User can create invites that other people can use to join the server.
    /// </summary>
    public bool CreateInvites => Has(ChannelPermission.CreateInvites);

    /// <summary>
    /// User can embed links in channels.
    /// </summary>
    public bool EmbedLinks => Has(ChannelPermission.SendEmbeds);

    /// <summary>
    /// User can upload files in channels.
    /// </summary>
    public bool UploadFiles => Has(ChannelPermission.UploadFiles);

    /// <summary>
    /// User can send masquerade messages with a custom name and avatar.
    /// </summary>
    public bool Masquerade => Has(ChannelPermission.Masquerade);

    /// <summary>
    /// User can add reactions to messages in channels.
    /// </summary>
    public bool AddReactions => Has(ChannelPermission.AddReactions);

    /// <summary>
    /// User can connect to voice channels.
    /// </summary>
    public bool VoiceConnect => Has(ChannelPermission.VoiceConnect);
    
    /// <summary>
    /// User can speak in voice channels.
    /// </summary>
    public bool VoiceSpeak => Has(ChannelPermission.VoiceSpeak);

    /// <summary>
    /// User can use video in voice channels.
    /// </summary>
    public bool VoiceVideo => Has(ChannelPermission.VoiceVideo);

    /// <summary>
    /// User can mute other users in voice channels.
    /// </summary>
    public bool VoiceMuteMembers => Has(ChannelPermission.VoiceMuteMembers);

    /// <summary>
    /// User can deafen other users in voice channels.
    /// </summary>
    public bool VoiceDeafenMembers => Has(ChannelPermission.VoiceDeafenMembers);

    /// <summary>
    /// User can move other users to different voice channels.
    /// </summary>
    public bool VoiceMoveMembers => Has(ChannelPermission.VoiceMoveMembers);

    /// <summary>
    /// Check if the server role override has a specific server permission. 
    /// </summary>
    /// <returns><see langword="bool" /></returns>
    public bool Has(ServerPermission permission)
    {
        ulong Flag = (ulong)permission;
        return (Raw & Flag) == Flag;
    }

    /// <summary>
    /// Check if the server role override has a specific channel permission. 
    /// </summary>
    /// <returns><see langword="bool" /></returns>
    public bool Has(ChannelPermission permission)
    {
        ulong Flag = (ulong)permission;
        return (Raw & Flag) == Flag;
    }
}