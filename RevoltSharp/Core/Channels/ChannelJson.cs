using Newtonsoft.Json;
using System;

namespace RevoltSharp
{
    public class ChannelJson
    {
        [JsonProperty("_id")]
        public string Id;

        [JsonProperty("channel_type")]
        public ChannelType ChannelType;

        [JsonProperty("nonce")]
        public string Nonce;

        [JsonProperty("user")]
        public string User;

        [JsonProperty("active")]
        public bool Active;

        [JsonProperty("recipients")]
        public string[] Recipients;

        [JsonProperty("last_message_id")]
        public string LastMessageId;

        [JsonProperty("icon")]
        public AttachmentJson Icon;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("owner")]
        public string Owner;

        [JsonProperty("permissions")]
        public int Permissions;

        [JsonProperty("default_permissions")]
        public int DefaultPermissions;

        [JsonProperty("server")]
        public string Server;

        [JsonProperty("nsfw")]
        public bool Nsfw;
    }
}
