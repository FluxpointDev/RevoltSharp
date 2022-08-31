using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp
{
    internal class EmojiJson
    {
        [JsonProperty("_id")]
        public string Id;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("parent")]
        public EmojiParentJson Parent;

        [JsonProperty("creator_id")]
        public string CreatorId;

        [JsonProperty("animated")]
        public bool Animated;

        [JsonProperty("nsfw")]
        public bool Nsfw;
    }
    internal class EmojiParentJson
    {
        [JsonProperty("id")]
        public string ServerId;
    }
}
