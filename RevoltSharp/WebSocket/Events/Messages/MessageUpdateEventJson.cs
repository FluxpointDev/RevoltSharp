using Newtonsoft.Json;
using Optionals;
using System;

namespace RevoltSharp.WebSocket.Events
{
    internal class MessageUpdateEventJson
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("channel")]
        public string Channel;

        [JsonProperty("data")]
        public MessageUpdatedJson Data;
    }
    internal class MessageUpdatedJson
    {
        [JsonProperty("content")]
        public Optional<string> Content;

        [JsonProperty("embeds")]
        public Optional<EmbedJson[]> Embeds;

        [JsonProperty("edited")]
        public DateTime Edited;
    }
}
