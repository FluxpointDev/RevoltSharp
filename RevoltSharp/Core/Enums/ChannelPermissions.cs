using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public class ChannelPermissions
    {
        public static ulong AllChannelPermissions = 0;

        public ulong RawAllowed { get; internal set; }
        public ulong RawDenied { get; internal set; }
        public ChannelPermissions(PermissionsJson permissions)
        {
            if (permissions == null)
                return;
            RawAllowed = permissions.Allowed;
            RawDenied = permissions.Denied;
        }

        public ChannelPermissions(ulong permissions)
        {
            RawAllowed = permissions;
        }

        internal bool Has(ChannelPermission permission)
        {
            ulong Flag = (ulong)permission;
            return !((RawDenied & Flag) == Flag);
        }
        public bool ViewChannel => Has(ChannelPermission.ViewChannel);
        public bool SendMessages => Has(ChannelPermission.SendMessages);
        public bool ManageMessages => Has(ChannelPermission.ManageMessages);
        public bool ManageChannel => Has(ChannelPermission.ManageChannel);
        public bool VoiceCall => Has(ChannelPermission.VoiceCall);
        public bool CreateInvite => Has(ChannelPermission.CreateInvite);
        public bool EmbedLinks => Has(ChannelPermission.EmbedLinks);
        public bool UploadFiles => Has(ChannelPermission.UploadFiles);
    }
}
