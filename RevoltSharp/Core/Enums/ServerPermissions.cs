using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public class ServerPermissions
    {
        public static ulong AllServerPermissions = 61503;
        public static ulong AllChannelPermissions = 247;
        public ServerPermissions(ulong[] permissions)
        {
            RawServer = permissions[0];
            RawChannel = permissions[1];
        }

        public ServerPermissions(Server server, ServerMember member)
        {
            if (server.OwnerId == member.Id)
            {
                RawServer = AllServerPermissions;
                RawChannel = AllChannelPermissions;
            }
            else
            {
                ulong resolvedServer = 0;
                ulong resolvedChannel = 0;
                foreach (Role r in member.Roles.Values)
                {
                    resolvedServer |= r.Permissions.RawServer;
                    resolvedChannel |= r.Permissions.RawChannel;
                }
                RawServer = resolvedServer;
                RawChannel = resolvedChannel;
            }
        }

        public ulong RawServer { get; internal set; }
        public ulong RawChannel { get; internal set; }

        public bool ViewChanels => Has(ServerPermission.ViewChannels);
        public bool ManageRoles => Has(ServerPermission.ManageRoles);
        public bool ManageChannels => Has(ServerPermission.ManageChannels);
        public bool ManageServer => Has(ServerPermission.ManageServer);
        public bool KickMembers => Has(ServerPermission.KickMembers);
        public bool BanMembers => Has(ServerPermission.BanMembers);
        public bool ChangeNickname => Has(ServerPermission.ChangeNickname);
        public bool ManageNicknames => Has(ServerPermission.ManageNicknames);
        public bool ChangeAvatar => Has(ServerPermission.ChangeAvatar);
        public bool ManageAvatars => Has(ServerPermission.RemoveAvatars);
        public bool SendMessages => Has(ChannelPermission.SendMessages);
        public bool ManageMessages => Has(ChannelPermission.ManageMessages);
        public bool VoiceCall => Has(ChannelPermission.VoiceCall);
        public bool CreateInvite => Has(ChannelPermission.CreateInvite);
        public bool EmbedLinks => Has(ChannelPermission.EmbedLinks);
        public bool UploadFiles => Has(ChannelPermission.UploadFiles);
        internal bool Has(ServerPermission permission)
        {
            ulong Flag = (ulong)permission;
            return (RawServer & Flag) == Flag;
        }
        internal bool Has(ChannelPermission permission)
        {
            ulong Flag = (ulong)permission;
            return (RawChannel & Flag) == Flag;
        }
    }
}
