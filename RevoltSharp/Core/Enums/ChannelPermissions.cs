namespace RevoltSharp;

/// <summary>
/// Permissions for the channel that members can or can't use
/// </summary>
public class ChannelPermissions
{
    public static ulong AllChannelPermissions = ulong.MaxValue;
    public Server Server { get; internal set; }

    public ulong RawAllowed { get; internal set; }
    public ulong RawDenied { get; internal set; }
    internal ChannelPermissions(Server server, PermissionsJson permissions)
    {
        if (server != null)
            Server = server;

        if (permissions == null)
            return;



        RawAllowed = permissions.Allowed;
        RawDenied = permissions.Denied;
    }

    internal ChannelPermissions(Server server, ulong allowed, ulong denied)
    {
        RawAllowed = allowed;
        RawDenied = denied;
    }

    public bool Has(ChannelPermission permission)
    {
        ulong Flag = (ulong)permission;
        if ((RawDenied & Flag) == Flag)
            return false;

        if (Server != null && Server.DefaultPermissions.Has(permission))
            return true;

        return ((RawAllowed & Flag) == Flag);
    }

    public bool ManageChannel => Has(ChannelPermission.ManageChannel);
    public bool ManagePermissions => Has(ChannelPermission.ManagePermissions);
    public bool ViewChanel => Has(ChannelPermission.ViewChannel);
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
}
