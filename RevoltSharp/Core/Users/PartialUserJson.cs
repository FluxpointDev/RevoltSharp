using Newtonsoft.Json;
using Optional;

namespace RevoltSharp
{
    internal class PartialUserJson
    {
        [JsonProperty("profile.content")]
        public Option<string> ProfileContent;

        [JsonProperty("profile.background")]
        public Option<AttachmentJson> ProfileBackground;

        public Option<UserStatusJson> status;

        public Option<AttachmentJson> avatar;
    }
}
