using Newtonsoft.Json;

namespace RevoltSharp
{
    internal class CreateInviteJson
    {
        [JsonProperty("_id")]
        public string Code;

        [JsonProperty("creator")]
        public string CreatorId;

        [JsonProperty("channel")]
        public string ChannelId;

        [JsonProperty("type")]
        public string ChannelType;
    }
    internal class InviteJson
    {
        [JsonProperty("code")]
        public string Code;

        [JsonProperty("channel_id")]
        public string ChannelId;

        [JsonProperty("channel_name")]
        public string ChannelName;

        [JsonProperty("channel_description")]
        public string ChannelDescription;

        [JsonProperty("user_name")]
        public string CreatorName;

        [JsonProperty("user_avatar")]
        public AttachmentJson CreatorAvatar;

        [JsonProperty("type")]
        public string ChannelType;
    }
}
