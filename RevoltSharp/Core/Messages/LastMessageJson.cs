using Newtonsoft.Json;

namespace RevoltSharp
{
    internal class LastMessageJson
    {
        [JsonProperty("_id")]
        public string id;
        public string author;
        [JsonProperty("short")]
        public string content;

        public LastMessage ToEntity()
        {
            return new LastMessage
            {
                Id = id,
                AuthorId = author,
                ContentPreview = content
            };
        }
    }
}
