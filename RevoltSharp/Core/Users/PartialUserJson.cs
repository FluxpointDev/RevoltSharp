using Newtonsoft.Json;
using Optionals;

namespace RevoltSharp
{

    internal class PartialUserJson
    {

        [JsonProperty("username")]
        public Optional<string> Username;

        [JsonProperty("discriminator")]
        public Optional<string> Discriminator;

        [JsonProperty("display_name")]
        public Optional<string> DisplayName;

        [JsonProperty("profile")]
        public Optional<ProfileJson> Profile;

        [JsonProperty("status")]
        public Optional<UserStatusJson> Status;

        [JsonProperty("avatar")]
        public Optional<AttachmentJson> Avatar;

        [JsonProperty("online")]
        public Optional<bool> Online;

        [JsonProperty("privileged")]
        public Optional<bool> Privileged;

        [JsonProperty("badges")]
        public Optional<ulong> Badges;

        [JsonProperty("flags")]
        public Optional<ulong> Flags;
    }
}