using Newtonsoft.Json;

namespace RevoltSharp
{
    public class UserJson
    {
        [JsonProperty("_id")]
        public string Id;

        [JsonProperty("username")]
        public string Username;

        [JsonProperty("avatar")]
        public AttachmentJson Avatar;

        [JsonProperty("badges")]
        public int Badges;

        [JsonProperty("status")]
        public UserStatusJson Status;

        [JsonProperty("profile")]
        public UserProfileJson Profile;

        [JsonProperty("bot")]
        public UserBotJson Bot;

        [JsonProperty("relationship")]
        public string Relationship;

        [JsonProperty("online")]
        public bool Online;
    }
    public class UserStatusJson
    {
        [JsonProperty("text")]
        public string Text;
    }
    public class UserBotJson
    {
        [JsonProperty("owner")]
        public string Owner;
    }
    public class UserProfileJson
    {
        [JsonProperty("content")]
        public string Content;
    }
}
