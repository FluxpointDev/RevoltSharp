using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Optionals;

namespace RevoltSharp
{
    internal class EmbedJson
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public EmbedType type;
        public int width;
        public int height;
        public string icon_url;
        public string url;
        public string title;
        public string site_name;
        public string description;
        public EmbedMediaJson image;
        public object media;
        public EmbedMediaJson video;
        public Optional<string> colour;
        public EmbedSpecialJson special;
    }
    internal class EmbedSpecialJson
    {
        [JsonConverter(typeof(StringEnumConverter)), JsonProperty("type")]
        public EmbedProviderType Type;
    }
    internal class EmbedMediaJson
    {
        public string url;
        public int width;
        public int height;
    }
}
