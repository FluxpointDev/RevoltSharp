using Newtonsoft.Json;
using System.Numerics;

namespace RevoltSharp
{
    public class RoleJson
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("permissions")]
        public ulong[] Permissions;

        [JsonProperty("hoist")]
        public bool Hoist;

        [JsonProperty("rank")]
        public BigInteger Rank;

        [JsonProperty("colour")]
        public string Colour;
    }
}
