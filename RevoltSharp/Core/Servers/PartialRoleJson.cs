using Newtonsoft.Json;
using Optional;
using System.Numerics;

namespace RevoltSharp
{
    public class PartialRoleJson
    {
        [JsonProperty("name")]
        public Option<string> Name;

        [JsonProperty("permissions")]
        public Option<ulong[]> Permissions;

        [JsonProperty("hoist")]
        public Option<bool> Hoist;

        [JsonProperty("rank")]
        public Option<BigInteger> Rank;

        [JsonProperty("colour")]
        public Option<string> Colour;
    }
}
