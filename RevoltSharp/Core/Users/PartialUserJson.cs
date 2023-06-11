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

    public Optional<bool> online;

    public Optional<bool> privileged;

    [JsonProperty("username")]
    public Optional<string> Username;

    [JsonProperty("display_name")]
    public Optional<string> DisplayName;

    [JsonProperty("discriminator")]
    public Optional<string> Discriminator;

    [JsonProperty("badges")]
    public Optional<ulong> Badges;

    [JsonProperty("flags")]
    public Optional<ulong> Flags;



}
