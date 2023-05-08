using Newtonsoft.Json;
using Optionals;

namespace RevoltSharp;

internal class PartialUserJson
{
    [JsonProperty("profile.content")]
    public Optional<string> ProfileContent;

    [JsonProperty("profile.background")]
    public Optional<AttachmentJson> ProfileBackground;

    public Optional<UserStatusJson> status;

    public Optional<AttachmentJson> avatar;

    public Optional<bool> privileged;

    [JsonProperty("username")]
    public Optional<string> Username;

    [JsonProperty("badges")]
    public Optional<ulong> Badges;

    [JsonProperty("flags")]
    public Optional<ulong> Flags;
}
