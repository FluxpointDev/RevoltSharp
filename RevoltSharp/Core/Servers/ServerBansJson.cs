using Newtonsoft.Json;

namespace RevoltSharp.Core.Servers;

internal class ServerBansJson
{
    [JsonProperty("users")]
    public ServerBanUserJson[]? Users;

    [JsonProperty("bans")]
    public ServerBanInfoJson[]? Bans;
}
internal class ServerBanUserJson
{
    [JsonProperty("id")]
    public string? Id;

    [JsonProperty("username")]
    public string? Username;

    [JsonProperty("avatar")]
    public AttachmentJson? Avatar;
}
internal class ServerBanInfoJson
{
    [JsonProperty("reason")]
    public string? Reason;

    [JsonProperty("_id")]
    public ServerBanIdJson? Ids;
}
internal class ServerBanIdJson
{
    [JsonProperty("user")]
    public string? UserId;
}
