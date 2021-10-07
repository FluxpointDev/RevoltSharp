using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public class SelfUser : User
    {
        public string Owner => BotData.Owner;

        public string ProfileBio { get; }

        public SelfUser(RevoltClient client, UserJson model)
            : base(client, model)
        {
            ProfileBio = model.Profile?.Content;
        }

        internal static SelfUser CreateSelf(User user)
            => new SelfUser(user.Client, user.Model);
    }
}
