using Newtonsoft.Json;

namespace RevoltSharp;

/// <summary>
/// Permissions for the server that members can or can't use
/// </summary>
public class ServerPermissions
{
    public static ulong AllServerPermissions = ulong.MaxValue;

    [JsonIgnore]
    public Server Server { get; internal set; }

    internal ServerPermissions(Server server, ulong permissions)
    {
        Server = server;
        RawServer = permissions;
    }

    internal ServerPermissions(Server server, ServerMember member)
    {
        Server = server;

        if (server != null && server.OwnerId == member.Id)
        {
            RawServer = ulong.MaxValue;
        }
        else
        {
            ulong resolvedServer = 0;
            foreach (Role r in member.InternalRoles.Values)
            {
                resolvedServer |= r.Permissions.RawServer;
            }
            RawServer = resolvedServer;
        }
    }

    public ulong RawServer { get; internal set; }

    public bool ManageChannels => Has(ChannelPermission.ManageChannel);
    public bool ManageServer => Has(ServerPermission.ManageServer);
    public bool ManagePermissions => Has(ServerPermission.ManagePermissions);
    public bool ManageRoles => Has(ServerPermission.ManageRoles);
    public bool ManageCustomisation => Has(ServerPermission.ManageCustomisation);
    public bool KickMembers => Has(ServerPermission.KickMembers);
    public bool BanMembers => Has(ServerPermission.BanMembers);
    public bool TimeoutMembers => Has(ServerPermission.TimeoutMembers);
    public bool AssignRoles => Has(ServerPermission.AssignRoles);
    public bool ChangeNickname => Has(ServerPermission.ChangeNickname);
    public bool ManageNicknames => Has(ServerPermission.ManageNicknames);
    public bool ChangeAvatar => Has(ServerPermission.ChangeAvatar);
    public bool ManageAvatars => Has(ServerPermission.ManageAvatars);
    public bool ViewChannels => Has(ChannelPermission.ViewChannel);
    public bool SendMessages => Has(ChannelPermission.SendMessages);
    public bool ManageMessages => Has(ChannelPermission.ManageMessages);
    public bool ManageWebhooks => Has(ChannelPermission.ManageWebhooks);
    public bool CreateInvites => Has(ChannelPermission.CreateInvites);
    public bool EmbedLinks => Has(ChannelPermission.SendEmbeds);
    public bool UploadFiles => Has(ChannelPermission.UploadFiles);
    public bool Masquerade => Has(ChannelPermission.Masquerade);
    public bool AddReactions => Has(ChannelPermission.AddReactions);
    public bool VoiceConnect => Has(ChannelPermission.VoiceConnect);
    public bool VoiceSpeak => Has(ChannelPermission.VoiceSpeak);
    public bool VoiceVideo => Has(ChannelPermission.VoiceVideo);
    public bool VoiceMuteMembers => Has(ChannelPermission.VoiceMuteMembers);
    public bool VoiceDeafenMembers => Has(ChannelPermission.VoiceDeafenMembers);
    public bool VoiceMoveMembers => Has(ChannelPermission.VoiceMoveMembers);

    public bool Has(ServerPermission permission)
    {
        ulong Flag = (ulong)permission;
        return (RawServer & Flag) == Flag;
    }

    public bool Has(ChannelPermission permission)
    {
        ulong Flag = (ulong)permission;
        return (RawServer & Flag) == Flag;
    }
}
