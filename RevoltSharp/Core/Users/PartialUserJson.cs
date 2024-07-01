using Newtonsoft.Json;
using Optionals;

namespace RevoltSharp;


internal class PartialUserJson
{

    [JsonProperty("username")]
    public Optional<string> Username { get; set; }

    [JsonProperty("discriminator")]
    public Optional<string> Discriminator { get; set; }

    [JsonProperty("display_name")]
    public Optional<string> DisplayName { get; set; }

    [JsonProperty("profile")]
    public Optional<ProfileJson> Profile { get; set; }

    [JsonProperty("status")]
    public Optional<UserStatusJson> Status { get; set; }

    [JsonProperty("avatar")]
    public Optional<AttachmentJson> Avatar { get; set; }

    [JsonProperty("online")]
    public Optional<bool> Online { get; set; }

    [JsonProperty("privileged")]
    public Optional<bool> Privileged { get; set; }

    [JsonProperty("badges")]
    public Optional<ulong> Badges { get; set; }

    [JsonProperty("flags")]
    public Optional<ulong> Flags { get; set; }
}