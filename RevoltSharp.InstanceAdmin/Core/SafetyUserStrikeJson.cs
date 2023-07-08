using Newtonsoft.Json;

namespace RevoltSharp;

internal class SafetyUserStrikeJson
{
    [JsonProperty("_id")]
    public string Id;

    [JsonProperty("user_id")]
    public string UserId;

    [JsonProperty("reason")]
    public string Reason;
}
