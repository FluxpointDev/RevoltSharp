using Newtonsoft.Json;
using System.Numerics;

namespace RevoltSharp
{
    internal class RoleJson
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("permissions")]
        public PermissionsJson Permissions;

        [JsonProperty("hoist")]
        public bool Hoist;

        [JsonProperty("rank")]
        public BigInteger Rank;

        [JsonProperty("colour")]
        public string Colour;
    }
    internal class PermissionsJson
    {
        [JsonProperty("a")]
        public ulong Allowed;

        [JsonProperty("d")]
        public ulong Denied;
    }
}
