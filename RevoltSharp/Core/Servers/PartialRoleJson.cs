using Newtonsoft.Json;
using Optional;
using Optional.Unsafe;

namespace RevoltSharp
{
    public class PartialRoleJson
    {
        [JsonProperty("name")]
        public Option<string> Name;

        [JsonProperty("permissions")]
        public Option<int[]> Permissions;

        [JsonProperty("hoist")]
        public Option<bool> Hoist;

        [JsonProperty("rank")]
        public Option<int> Rank;

        [JsonProperty("colour")]
        public Option<string> Colour;
    }
}
