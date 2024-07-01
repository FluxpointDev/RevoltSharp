using Newtonsoft.Json;
using Optionals;
using System.Numerics;

namespace RevoltSharp;


internal class PartialRoleJson
{
    [JsonProperty("name")]
    public Optional<string> Name { get; set; }

    [JsonProperty("permissions")]
    public Optional<PermissionsJson> Permissions { get; set; }

    [JsonProperty("hoist")]
    public Optional<bool> Hoist { get; set; }

    [JsonProperty("rank")]
    public Optional<BigInteger> Rank { get; set; }

    [JsonProperty("colour")]
    public Optional<string> Colour { get; set; }
}