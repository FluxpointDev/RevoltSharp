using Newtonsoft.Json;

namespace RevoltSharp;


internal class QueryJson
{
    [JsonProperty("revolt")]
    public string? RevoltVersion;

    [JsonProperty("features")]
    public QueryFeaturesJson? ServerFeatures;

    [JsonProperty("ws")]
    public string? WebsocketUrl;

    [JsonProperty("app")]
    public string? AppUrl;
}
internal class QueryFeaturesJson
{
    [JsonProperty("autumn")]
    public QueryAutumnJson? ImageServer;

    [JsonProperty("january")]
    public QueryJanuaryJson? JanuaryServer;

    [JsonProperty("voso")]
    public QueryVosoJson? VoiceServer;

    public QueryCaptchaJson? captcha;

    public bool email;
    public bool invite_only;
}
internal class QueryCaptchaJson
{
    public bool enabled;
}
internal class QueryAutumnJson
{
    public string? url;
}
internal class QueryJanuaryJson
{
    public string? url;
}
internal class QueryVosoJson
{
    public string? url;
    public string? ws;
}