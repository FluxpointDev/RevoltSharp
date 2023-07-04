using Newtonsoft.Json;

namespace RevoltSharp.Rest.Requests
{

    internal class QueryRequest
    {
        [JsonProperty("revolt")]
        public string? revoltVersion;

        [JsonProperty("features")]
        public QueryFeatures? serverFeatures;

        [JsonProperty("ws")]
        public string? websocketUrl;
    }
    internal class QueryFeatures
    {
        [JsonProperty("autumn")]
        public QueryAutumn? imageServer;

        [JsonProperty("january")]
        public QueryJanuary? otherServer;

        [JsonProperty("voso")]
        public QueryVoso? voiceServer;
    }
    internal class QueryAutumn
    {
        public string? url;
    }
    internal class QueryJanuary
    {
        public string? url;
    }
    internal class QueryVoso
    {
        public string? url;
        public string? ws;
    }
}