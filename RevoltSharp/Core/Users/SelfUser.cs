using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public class SelfUser : User
    {
        public string Owner
            => BotData.Owner;

        public string ProfileBio { get { return base.ProfileBio; } internal set { base.ProfileBio = value; } }
    }
}
