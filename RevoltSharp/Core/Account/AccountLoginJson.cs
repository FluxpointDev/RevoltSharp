using Newtonsoft.Json;

namespace RevoltSharp;

internal class AccountLoginJson
{
    [JsonProperty("result")]
    public string Result { get; set; }

    [JsonProperty("_id")]
    public string Id { get; set; }

    [JsonProperty("user_id")]
    public string UserId { get; set; }

    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("ticket")]
    public string Ticket { get; set; }

    [JsonProperty("allowed_methods")]
    public string[] AllowedMethods { get; set; }
}
