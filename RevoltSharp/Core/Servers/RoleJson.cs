using Newtonsoft.Json;
using System.Numerics;

namespace RevoltSharp;


internal class RoleJson
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("permissions")]
    public PermissionsJson Permissions { get; set; } = null!;

    [JsonProperty("hoist")]
    public bool Hoist { get; set; }

    [JsonProperty("rank")]
    public BigInteger Rank { get; set; }

    [JsonProperty("colour")]
    public string? Colour { get; set; }
}
internal class PermissionsJson
{
    [JsonProperty("a")]
    public ulong Allowed { get; set; }

    [JsonProperty("d")]
    public ulong Denied { get; set; }
}