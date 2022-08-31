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
        internal ChannelPermissions(PermissionsJson permissions)
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

        public bool ManageChannel => Has(ChannelPermission.ManageChannel);
        public bool ManagePermissions => Has(ChannelPermission.ManagePermissions);
        public bool ViewChanel => Has(ChannelPermission.ViewChannel);
        public bool ReadMessageHistory => Has(ChannelPermission.ReadMessageHistory);
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
}
