using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public class SelfUser : User
    {
        public string OwnerId => BotData.Owner;

        public User Owner
            => Client.GetUser(OwnerId);

        public string ProfileBio { get; internal set; }

        public Attachment Background { get; internal set; }

        public SelfUser(RevoltClient client, UserJson model)
            : base(client, model)
        {
            ProfileBio = model.Profile?.Content;
            Background = model.Profile?.Background != null ? new Attachment(client, model.Profile.Background) : null;
        }

        internal static SelfUser CreateSelf(User user)
            => new SelfUser(user.Client, user.Model);
    }
}
