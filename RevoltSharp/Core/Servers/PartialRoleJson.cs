using Newtonsoft.Json;
using Optionals;
using System.Numerics;

namespace RevoltSharp;

internal class PartialRoleJson
{
    [JsonProperty("name")]
    public Optional<string> Name;

    [JsonProperty("permissions")]
    public Optional<PermissionsJson> Permissions;

    [JsonProperty("hoist")]
    public Optional<bool> Hoist;

    [JsonProperty("rank")]
    public Optional<BigInteger> Rank;

    [JsonProperty("colour")]
    public Optional<string> Colour;
}
