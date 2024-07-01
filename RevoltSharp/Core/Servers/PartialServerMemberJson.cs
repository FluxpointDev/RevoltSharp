using Newtonsoft.Json;
using Optionals;
using System;

namespace RevoltSharp;


internal class PartialServerMemberJson
{
    [JsonProperty("nickname")]
    public Optional<string> Nickname { get; set; }

    [JsonProperty("avatar")]
    public Optional<AttachmentJson> Avatar { get; set; }

    [JsonProperty("roles")]
    public Optional<string[]> Roles { get; set; }

    [JsonProperty("timeout")]
    public Optional<DateTime> Timeout { get; set; }

    public bool ClearTimeout { get; set; }
}