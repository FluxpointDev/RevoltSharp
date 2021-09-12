using Newtonsoft.Json;

namespace RevoltSharp
{
    internal class UserJson
    {
        [JsonProperty("_id")]
        public string id;
        public string username;
        public AttachmentJson avatar;
        public int badges;
        public UserStatusJson status;
        public UserProfileJson profile;
        public UserBotJson bot;
        public string relationship;
        public bool online;

        internal User ToEntity()
        {
            return new User
            {
                Id = id,
                Username = username,
                BotData = bot != null ? new BotData { Owner = bot.owner } : null,
                Avatar = avatar != null ? avatar.ToEntity() : null,
                Badges = new UserBadges() { Raw = badges },
                IsOnline = online,
                ProfileBio = profile != null ? profile.content : null,
                Relationship = relationship,
                Status = status != null ? status.text : null
            };
        }
    }
    internal class UserStatusJson
    {
        public string text;
    }
    internal class UserBotJson
    {
        public string owner;
    }
    internal class UserProfileJson
    {
        public string content;
    }
}
