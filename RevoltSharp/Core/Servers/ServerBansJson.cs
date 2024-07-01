using Newtonsoft.Json;

namespace RevoltSharp;


internal class ServerBansJson
{
    [JsonProperty("users")]
    public ServerBanUserJson[] Users { get; set; } = null!;

    [JsonProperty("bans")]
    public ServerBanInfoJson[] Bans { get; set; } = null!;
}
internal class ServerBanUserJson
{
    [JsonProperty("id")]
    public string Id { get; set; } = null!;

    [JsonProperty("username")]
    public string Username { get; set; } = null!;

    [JsonProperty("avatar")]
    public AttachmentJson? Avatar { get; set; }

    [JsonProperty("discriminator")]
    public string Discriminator { get; set; } = null!;
}

internal class ServerBanInfoJson
{
    [JsonProperty("reason")]
    public string? Reason { get; set; }

    [JsonProperty("_id")]
    public ServerBanIdJson Id { get; set; } = null!;
}
internal class ServerBanIdJson
{
    [JsonProperty("user")]
    public string UserId { get; set; } = null!;
}