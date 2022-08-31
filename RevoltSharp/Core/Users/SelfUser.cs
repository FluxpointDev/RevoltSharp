using System;

namespace RevoltSharp
{
    public class SelfUser : User
    {
        public string OwnerId => BotData.Owner;

        public User Owner
            => Client.GetUser(OwnerId);

        public string ProfileBio { get; internal set; }

        public Attachment Background { get; internal set; }

        internal SelfUser(RevoltClient client, UserJson model)
            : base(client, model)
        {

            ProfileBio = model.Profile?.Content;
            Background = model.Profile?.Background != null ? new Attachment(client, model.Profile.Background) : null;
        }


        internal new SelfUser Clone()
        {
            return (SelfUser)this.MemberwiseClone();
        }

    }
}
