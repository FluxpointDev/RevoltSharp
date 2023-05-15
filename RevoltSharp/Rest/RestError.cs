using Newtonsoft.Json;

namespace RevoltSharp.Rest;
public class RestError
{
    [JsonProperty("type")]
    public RevoltErrorType Type = RevoltErrorType.Unknown;

    public bool IsMissingPermission => !string.IsNullOrEmpty(Permission);

    [JsonProperty("permission")]
    public string Permission;
}
