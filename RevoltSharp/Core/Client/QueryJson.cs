using Newtonsoft.Json;

namespace RevoltSharp;


internal class QueryJson
{
    [JsonProperty("revolt")]
    public string? RevoltVersion { get; set; }

    [JsonProperty("features")]
    public QueryFeaturesJson? ServerFeatures { get; set; }

    [JsonProperty("ws")]
    public string? WebsocketUrl { get; set; }

    [JsonProperty("app")]
    public string? AppUrl { get; set; }
}
internal class QueryFeaturesJson
{
    [JsonProperty("autumn")]
    public QueryAutumnJson? ImageServer { get; set; }

    [JsonProperty("january")]
    public QueryJanuaryJson? JanuaryServer { get; set; }

    [JsonProperty("voso")]
    public QueryVosoJson? VoiceServer { get; set; }

    public QueryCaptchaJson? captcha { get; set; }

    public bool email { get; set; }
    public bool invite_only { get; set; }
}
internal class QueryCaptchaJson
{
    public bool enabled { get; set; }
}
internal class QueryAutumnJson
{
    public string? url { get; set; }
}
internal class QueryJanuaryJson
{
    public string? url { get; set; }
}
internal class QueryVosoJson
{
    public string? url { get; set; }
    public string? ws { get; set; }
}