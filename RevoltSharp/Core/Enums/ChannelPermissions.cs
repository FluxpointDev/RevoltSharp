using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public class ChannelPermissions
    {
        public ChannelPermissions(ulong permission)
        {
            RawValue = permission;
        }
        public ulong RawValue { get; internal set; }

        public bool ViewChannel => Has(ChannelPermission.ViewChannel);
        public bool SendMessages => Has(ChannelPermission.SendMessages);
        public bool ManageMessages => Has(ChannelPermission.ManageMessages);
        public bool ManageChannel => Has(ChannelPermission.ManageChannel);
        public bool VoiceCall => Has(ChannelPermission.VoiceCall);
        public bool CreateInvite => Has(ChannelPermission.CreateInvite);
        public bool EmbedLinks => Has(ChannelPermission.EmbedLinks);
        public bool UploadFiles => Has(ChannelPermission.UploadFiles);

        internal bool Has(ChannelPermission permission)
        {
            ulong Flag = (ulong)permission;
            return (RawValue & Flag) == Flag;
        }
    }
}
