using Newtonsoft.Json;

namespace RevoltSharp.Rest;
internal class RestError
{
    [JsonProperty("type")]
    public RevoltErrorType Type = RevoltErrorType.Unknown;

    [JsonProperty("permission")]
    public string Permission;
}
