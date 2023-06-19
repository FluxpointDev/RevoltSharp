using Newtonsoft.Json;

namespace RevoltSharp;

internal class ServerBansJson
{
    [JsonProperty("users")]
    public ServerBanUserJson[] Users = null!;

    [JsonProperty("bans")]
    public ServerBanInfoJson[] Bans = null!;
}
internal class ServerBanUserJson
{
    [JsonProperty("id")]
    public string Id = null!;

    [JsonProperty("username")]
    public string Username = null!;

    [JsonProperty("avatar")]
    public AttachmentJson? Avatar;
}

internal class ServerBanInfoJson
{
    [JsonProperty("reason")]
    public string? Reason;

    [JsonProperty("_id")]
    public ServerBanIdJson Id = null!;
}
internal class ServerBanIdJson
{
    [JsonProperty("user")]
    public string? UserId;
}
