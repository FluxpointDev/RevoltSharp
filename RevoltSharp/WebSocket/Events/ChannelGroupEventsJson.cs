using Newtonsoft.Json;

namespace RevoltSharp.WebSocket;

internal class ChannelGroupJoinEventJson
{
    [JsonProperty("id")]
    public string? ChannelId;

    [JsonProperty("user")]
    public string? UserId;
}
internal class ChannelGroupLeaveEventJson
{
	[JsonProperty("id")]
	public string? ChannelId;

	[JsonProperty("user")]
	public string? UserId;
}
