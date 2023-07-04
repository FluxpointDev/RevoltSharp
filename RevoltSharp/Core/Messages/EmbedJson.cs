using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Optionals;

namespace RevoltSharp
{

    internal class EmbedJson
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public EmbedType type { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string? icon_url { get; set; }
        public string? url { get; set; }
        public string? title { get; set; }
        public string? site_name { get; set; }
        public string? description { get; set; }
        public EmbedMediaJson? image { get; set; }
        public object? media { get; set; }
        public EmbedMediaJson? video { get; set; }
        public Optional<string> colour { get; set; }
        public EmbedSpecialJson? special { get; set; }
    }
    internal class EmbedSpecialJson
    {
        [JsonConverter(typeof(StringEnumConverter)), JsonProperty("type")]
        public EmbedProviderType Type { get; set; }
    }
    internal class EmbedMediaJson
    {
        [JsonProperty("url")]
        public string Url { get; set; } = null!;

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }
}